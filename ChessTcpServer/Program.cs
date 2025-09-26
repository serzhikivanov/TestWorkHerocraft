using ChessTcpServer.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<IKnightMoveCalcService, KnightMoveCalcService>();
        services.AddHostedService<SocketListenerService>();
    })
    .Build();

await host.RunAsync();
