using ChessTcpServer.Configuration;
using ChessTcpServer.Parsers;
using Microsoft.Extensions.Options;

namespace ChessTcpServer.Services;

public class SocketListenerServiceHttp : SocketListenerService
{
    public SocketListenerServiceHttp(
        ILogger<SocketListenerServiceHttp> logger,
        IKnightMoveCalcService calcService,
        IOptions<ChessServerSettings> options)
        : base(logger, calcService, options,
               new ChessParserHttp<KnightRequest>(),
               options.Value.Url,
               int.Parse(options.Value.Port))
    { }
}
