using ChessTcpServer.Configuration;
using ChessTcpServer.Parsers;
using Microsoft.Extensions.Options;

namespace ChessTcpServer.Services;

public class SocketListenerServiceRaw : SocketListenerService
{
    public SocketListenerServiceRaw(
        ILogger<SocketListenerServiceRaw> logger,
        IKnightMoveCalcService calcService,
        IOptions<ChessServerSettings> options)
        : base(logger, calcService, options,
               new ChessParserRaw(),
               options.Value.UrlRaw,
               int.Parse(options.Value.PortRaw))
    { }
}
