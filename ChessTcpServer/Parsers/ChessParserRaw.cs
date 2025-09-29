using ChessTcpServer.Services;
using System.Text.Json;

namespace ChessTcpServer.Parsers;

public class ChessParserRaw : IChessParser<KnightRequest>
{
    public KnightRequest ParseRequest(string source)
        => JsonSerializer.Deserialize<KnightRequest>(source)!;

    public string ParseResponse(string source) 
        => source; 
}
