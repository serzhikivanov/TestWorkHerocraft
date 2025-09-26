using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ChessTcpServer.Services;

public class SocketListenerService : BackgroundService
{
    private ILogger<SocketListenerService> _logger;
    private IKnightMoveCalcService _knightMoveCalcService;
    private int _port = 5001;

    public SocketListenerService(ILogger<SocketListenerService> logger, IKnightMoveCalcService knightMoveCalcService)
    {
        _logger = logger;
        _knightMoveCalcService = knightMoveCalcService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("localhost");
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint ipEndPoint = new(ipAddress, _port);

        using (Socket listener = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
        {
            listener.Bind(ipEndPoint);
            listener.Listen(100);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var handler = await listener.AcceptAsync(stoppingToken);
                    var jsonBody = await ReadHttpRequestBodyAsync(handler, stoppingToken);
                    _logger.LogInformation("Received JSON: {0}", jsonBody);

                    var payload = JsonSerializer.Deserialize<KnightRequest>(jsonBody);

                    // ToDo: handle errors here

                    string[]? knightMovePath = _knightMoveCalcService.CalcKnightPath(payload.from, payload.to);
                    var respBody = JsonSerializer.Serialize(knightMovePath);
                    _logger.LogInformation("Serialized response: {0}", respBody);
                    await SendResponse(handler, 200, respBody, stoppingToken);

                    // В этом упрощенном случае keep-alive не нужен, закрываю соединение
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
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
    }

    private async Task<string> ReadHttpRequestBodyAsync(Socket handler, CancellationToken ct)
    {
        var headerBuffer = new List<byte>();
        var bodyBuffer = new List<byte>();
        var temp = new byte[1024];

        // Step 1: read headers until we see "\r\n\r\n"
        while (true)
        {
            int bytes = await handler.ReceiveAsync(temp, SocketFlags.None, ct);
            if (bytes == 0) throw new Exception("Client disconnected");

            headerBuffer.AddRange(temp[..bytes]);
            string headerText = Encoding.UTF8.GetString(headerBuffer.ToArray());

            int headerEnd = headerText.IndexOf("\r\n\r\n", StringComparison.Ordinal);
            if (headerEnd >= 0)
            {
                // we found the end of headers
                string headersOnly = headerText[..headerEnd];
                // parse Content-Length
                int contentLength = 0;
                foreach (var line in headersOnly.Split("\r\n"))
                {
                    if (line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = line.Split(':', 2);
                        contentLength = int.Parse(parts[1].Trim());
                    }
                }

                // Step 2: figure out if we already received part of the body
                int alreadyRead = headerBuffer.Count - (headerEnd + 4);
                if (alreadyRead > 0)
                    bodyBuffer.AddRange(headerBuffer.GetRange(headerEnd + 4, alreadyRead));

                // Step 3: read remaining body bytes if needed
                while (bodyBuffer.Count < contentLength)
                {
                    bytes = await handler.ReceiveAsync(temp, SocketFlags.None, ct);
                    if (bytes == 0) break;
                    bodyBuffer.AddRange(temp[..bytes]);
                }

                return Encoding.UTF8.GetString(bodyBuffer.ToArray());
            }
        }
    }

    private async Task SendResponse(Socket handler, int errorCode, string respBody, CancellationToken stoppingToken)
    {
        string response =
            "HTTP/1.1 200 OK\r\n" +
            "Content-Type: application/json\r\n" +
            $"Content-Length: {Encoding.UTF8.GetByteCount(respBody)}\r\n" + 
            "\r\n" +                                      
            respBody;

        // Convert to bytes and send
        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
        await handler.SendAsync(responseBytes, SocketFlags.None, stoppingToken);
    }
}

public record KnightRequest(string from, string to);