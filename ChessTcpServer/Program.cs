using ChessTcpServer.Configuration;
using ChessTcpServer.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<ChessServerSettings>(
            context.Configuration.GetSection("ChessServerSettings"));

        services.AddTransient<IKnightMoveCalcService, KnightMoveCalcService>();
        services.AddHostedService<SocketListenerService>();
    })
    .Build();

await host.RunAsync();
