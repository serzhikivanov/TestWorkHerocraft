namespace ChessTcpServer.Parsers
{
    public interface IChessParser<T>
    {
        public T ParseRequest(string source);

        public string ParseResponse(string source);
    }
}
