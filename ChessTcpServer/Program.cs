using ChessTcpServer.Configuration;
using ChessTcpServer.Services;

// за ненадобностью большей части стандартного кода для ASP .NET Core в этом случае, переделал на дженерик хост, которого в этом случае достаточно, чтобы
// запустить сервисом требуемый сокет сервер и использовать Dependency Injection
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<ChessServerSettings>(
            context.Configuration.GetSection("ChessServerSettings"));

        services.AddTransient<IKnightMoveCalcService, KnightMoveCalcService>();
        services.AddHostedService<SocketListenerServiceHttp>();
        services.AddHostedService<SocketListenerServiceRaw>();
    })
    .Build();

await host.RunAsync();
