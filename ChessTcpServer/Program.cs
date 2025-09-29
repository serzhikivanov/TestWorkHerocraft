using ChessTcpServer.Configuration;
using ChessTcpServer.Services;

// �� ������������� ������� ����� ������������ ���� ��� ASP .NET Core � ���� ������, ��������� �� �������� ����, �������� � ���� ������ ����������, �����
// ��������� �������� ��������� ����� ������ � ������������ Dependency Injection
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
