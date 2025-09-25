using Microsoft.AspNetCore.Mvc;

namespace ChessSrvClient.Controllers;

[ApiController]
[Route("[controller]")]
public class ChessMoveController : ControllerBase
{
    private readonly ILogger<ChessMoveController> _logger;

    public ChessMoveController(ILogger<ChessMoveController> logger)
    {
        // Мне нравится у конструкторов классический вид со скобками, хотя в случае 1 строки логики можно
        // сделать и "public ChessMoveController(ILogger<ChessMoveController> logger) => _logger = logger;"
        _logger = logger;
    }

    [HttpGet(Name = "GetHorseMoves")]
    public string Get([FromQuery] string startCell, [FromQuery] string endCell)
    {
        return "[Sample return]";
    }
}
