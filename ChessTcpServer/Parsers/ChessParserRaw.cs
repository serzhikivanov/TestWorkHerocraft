using ChessTcpServer.Services;
using System.Text.Json;

namespace ChessTcpServer.Parsers;

public class ChessParserRaw<T> : IChessParser<T>
{
    public T ParseRequest(string source)
        => JsonSerializer.Deserialize<T>(source)!;

    public string ParseResponse(string source) 
        => source; 
}
