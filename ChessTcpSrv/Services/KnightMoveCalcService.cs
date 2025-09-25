namespace ChessTcpSrv.Services
{
    internal class KnightMoveCalcService
    {
        // Все возможные ходы коня по координатам (x,y)
        private static readonly (int dx, int dy)[] KnightMoves = new (int, int)[]
        {
            (2, 1), (1, 2), (-1, 2), (-2, 1), (-2, -1), (-1, -2), (1, -2), (2, -1)
        };

        public string[] CalcKnightPath(string startCell, string endCell)
        {
            if (!IsValidCell(startCell) || !IsValidCell(endCell))
                return Array.Empty<string>();

            if (startCell == endCell)
                return [startCell];

            var start = CellToCoords(startCell);
            var end = CellToCoords(endCell);

            // Поиск минимального пути конем (BFS)
            var queue = new Queue<(int x, int y)>();
            queue.Enqueue(start);

            var visited = new bool[8, 8];
            visited[start.x, start.y] = true;

            // Для восстановления пути храним откуда мы пришли
            var parent = new (int x, int y)?[8, 8];

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == end)
                {
                    // Восстановление пути
                    var path = new List<string>();
                    var cur = current;
                    while (cur != start)
                    {
                        path.Add(CoordsToCell(cur));
                        cur = parent[cur.x, cur.y].Value;
                    }
                    path.Add(CoordsToCell(start));
                    path.Reverse();
                    return path.ToArray();
                }

                foreach (var move in KnightMoves)
                {
                    int nx = current.x + move.dx;
                    int ny = current.y + move.dy;

                    if (nx >= 0 && ny >= 0 && nx < 8 && ny < 8 && !visited[nx, ny])
                    {
                        visited[nx, ny] = true;
                        parent[nx, ny] = current;
                        queue.Enqueue((nx, ny));
                    }
                }
            }

            // Пути не найдено
            return Array.Empty<string>();
        }

        private static bool IsValidCell(string cell)
        {
            if (cell.Length != 2) 
                return false;
            
            char file = char.ToLower(cell[0]);
            char rank = cell[1];

            return file >= 'a' && file <= 'h' && rank >= '1' && rank <= '8';
        }

        private static (int x, int y) CellToCoords(string cell)
        {
            // x — столбец, 0 для 'a', до 7 для 'h'
            // y — строка, 0 для '1', до 7 для '8'
            int x = cell[0] - 'a';
            int y = cell[1] - '1';
            return (x, y);
        }

        private static string CoordsToCell((int x, int y) coords)
        {
            char file = (char)('a' + coords.x);
            char rank = (char)('1' + coords.y);
            return $"{file}{rank}";
        }
    }
}
