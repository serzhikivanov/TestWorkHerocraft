using ChessTcpServer.Services;
using System.Text;
using System.Text.Json;

namespace ChessTcpServer.Parsers;

public class ChessParserHttp : IChessParser<KnightRequest>
{
    public KnightRequest ParseRequest(string source)
    {
        var idx = source.IndexOf("\r\n\r\n", StringComparison.Ordinal);
        if (idx < 0) 
            throw new InvalidOperationException("Bad HTTP request");

        var body = source[(idx + 4)..];
        return JsonSerializer.Deserialize<KnightRequest>(body)!;
    }

    public string ParseResponse(string source)
    {
        // Не использую синтаксис "ParseResponse(string source) => ..." только для визуальной однородности в коде класса
        return $"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: {Encoding.UTF8.GetByteCount(source)}\r\n\r\n{source}";
    }
}
