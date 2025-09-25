using Microsoft.AspNetCore.Mvc;

namespace ChessSrvClient.Controllers;

[ApiController]
[Route("[controller]")]
public class ChessMoveController : ControllerBase
{
    private readonly ILogger<ChessMoveController> _logger;

    public ChessMoveController(ILogger<ChessMoveController> logger)
    {
        // ��� �������� � ������������� ������������ ��� �� ��������, ���� � ������ 1 ������ ������ �����
        // ������� � "public ChessMoveController(ILogger<ChessMoveController> logger) => _logger = logger;"
        _logger = logger;
    }

    [HttpGet(Name = "GetHorseMoves")]
    public string Get([FromQuery] string startCell, [FromQuery] string endCell)
    {
        return "[Sample return]";
    }
}
