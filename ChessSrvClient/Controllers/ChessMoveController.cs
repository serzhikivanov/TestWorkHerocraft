using ChessSrvClient.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

    [HttpGet(Name = "GetKnightMoves")]
    public async Task<string> Get([FromQuery] string startCell, [FromQuery] string endCell)
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

        // Читаем строковый ответ от вашего TCP-сервера
        string responseBody = await response.Content.ReadAsStringAsync();

        return responseBody;
    }
}
