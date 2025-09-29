using ChessTcpServer.Configuration;
using ChessTcpServer.Parsers;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ChessTcpServer.Services;

public abstract class SocketListenerService : BackgroundService
{
    private readonly ILogger<SocketListenerService> _logger;
    private readonly IKnightMoveCalcService _calcService;
    private readonly IOptions<ChessServerSettings> _options;
    private readonly IChessParser<KnightRequest> _parser;
    private readonly string _url;
    private readonly int _port;

    protected SocketListenerService(
        ILogger<SocketListenerService> logger,
        IKnightMoveCalcService calcService,
        IOptions<ChessServerSettings> options,
        IChessParser<KnightRequest> parser,
        string url,
        int port)
    {
        _logger = logger;
        _calcService = calcService;
        _options = options;
        _parser = parser;
        _url = url;
        _port = port;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(_url);
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint ipEndPoint = new(ipAddress, _port);

        using var listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(ipEndPoint);
        listener.Listen(100);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Listening on {0}:{1}", _url, _port);
                var handler = await listener.AcceptAsync(stoppingToken);
                string request = await ReadAllAsync(handler, stoppingToken);
                _logger.LogInformation("Received: {0}", request);
                var payload = _parser.ParseRequest(request);

                var moves = _calcService.CalcKnightPath(payload.from, payload.to);
                var jsonResp = JsonSerializer.Serialize(moves);
                string responseText = _parser.ParseResponse(jsonResp);
                await handler.SendAsync(Encoding.UTF8.GetBytes(responseText), SocketFlags.None, stoppingToken);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
        catch (OperationCanceledException) 
        { 
            /* нормальное завершение */ 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TCP listener failed");
        }
        finally
        {
            _logger.LogInformation("TCP server stopped");
        }
    }

    private static async Task<string> ReadAllAsync(Socket handler, CancellationToken ct)
    {
        var buffer = new byte[4096];
        var sb = new StringBuilder();
        while (true)
        {
            int bytes = await handler.ReceiveAsync(buffer, SocketFlags.None, ct);
            if (bytes == 0) break;
            sb.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
            // реализация в парсере не требует знать Content-Length заранее
            if (!handler.Poll(10_000, SelectMode.SelectRead))
                break;
        }
        return sb.ToString();
    }
}

public record KnightRequest(string from, string to);