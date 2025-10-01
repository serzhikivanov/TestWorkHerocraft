using ChessSrvClient.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ChessSrvClient.Controllers;

[ApiController]
[Route("[controller]")]
public class ChessMoveController : ControllerBase
{
    private readonly ILogger<ChessMoveController> _logger;
    private readonly HttpClient _httpClient;
    private readonly IOptions<ChessClientSettings> _options;

    public ChessMoveController(ILogger<ChessMoveController> logger, IOptions<ChessClientSettings> options)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        _options = options;
    }

    [HttpPost(Name = "GetKnightMoves")]
    public async Task<string> GetKnightMoves([FromQuery] string startCell, [FromQuery] string endCell)
    {
        var requestObj = new
        {
            from = startCell,
            to = endCell
        };

        var json = JsonSerializer.Serialize(requestObj);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        _logger.LogInformation("Sending request {0} to {1}", json, _options.Value.Url);

        var response = await _httpClient.PostAsync(_options.Value.Url, content);

        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("Received response {0}", response);

        return responseBody;
    }

    [HttpPost("GetKnightMovesRaw")]
    public async Task<string> GetKnightMovesRaw([FromQuery] string startCell, [FromQuery] string endCell)
    {
        var requestObj = new { from = startCell, to = endCell };
        var json = JsonSerializer.Serialize(requestObj);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        _logger.LogInformation("Sending RAW request {0} to {1}",
            json, _options.Value.UrlRaw);

        using var client = new TcpClient();
        await client.ConnectAsync(_options.Value.UrlRaw, int.Parse(_options.Value.PortRaw));
        var stream = client.GetStream();
        var data = Encoding.UTF8.GetBytes(json);
        await stream.WriteAsync(data);
        await stream.FlushAsync();

        var buffer = new byte[4096];
        int read = await stream.ReadAsync(buffer);
        return Encoding.UTF8.GetString(buffer, 0, read);
    }
}
