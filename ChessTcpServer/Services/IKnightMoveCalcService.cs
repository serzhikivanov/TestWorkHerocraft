namespace ChessTcpServer.Services
{
    public interface IKnightMoveCalcService
    {
        string[] CalcKnightPath(string startCell, string endCell);
    }
}
