using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ChessSrvClient.Controllers;

[ApiController]
[Route("[controller]")]
public class ChessMoveController : ControllerBase
{
    private readonly ILogger<ChessMoveController> _logger;
    private readonly HttpClient _httpClient;

    public ChessMoveController(ILogger<ChessMoveController> logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();
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
        var response = await _httpClient.PostAsync("http://localhost:5001/", content);
        //var response = await _httpClient.PostAsync("http://servermicroservice:5001/", content);

        response.EnsureSuccessStatusCode();

        // Читаем строковый ответ от вашего TCP-сервера
        string responseBody = await response.Content.ReadAsStringAsync();

        return responseBody;
    }
}
