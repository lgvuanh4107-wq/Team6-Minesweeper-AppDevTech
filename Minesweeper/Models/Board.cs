namespace Minesweeper.Models;

public class Board
{
    private readonly Cell[,] _cells;
    private readonly Difficulty _difficulty;
    private int _revealedCount;
    private int _flaggedCount;
    private readonly int _totalSafeCells;

    public int Width => _difficulty.Width;
    public int Height => _difficulty.Height;
    public int MineCount => _difficulty.Mines;

    public Cell this[int x, int y] => _cells[x, y];

    public bool IsStarted { get; private set; }
    public bool IsOver { get; private set; }
    public bool IsWin { get; private set; }

    public int FlagsRemaining => MineCount - _flaggedCount;
    public int ProgressPercent => IsStarted ? (_revealedCount * 100) / _totalSafeCells : 0;

    public (int X, int Y) LastHit { get; private set; }

    public event Action? GameStarted;
    public event Action? GameWon;
    public event Action? GameLost;

    // Khởi tạo bàn cờ với kích thước và số mìn từ độ khó
    public Board(Difficulty d)
    {
        _difficulty = d;
        _cells = new Cell[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _cells[x, y] = new Cell();
            }
        }
        _totalSafeCells = Width * Height - MineCount;
    }

    // Kiểm tra tọa độ có nằm trong bàn cờ không
    private bool IsValid(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    // Lấy danh sách tọa độ các ô xung quanh hợp lệ
    private IEnumerable<(int X, int Y)> GetNeighbors(int cx, int cy)
    {
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
                if ((dx != 0 || dy != 0) && IsValid(cx + dx, cy + dy))
                    yield return (cx + dx, cy + dy);
    }

    // Đặt mìn và tính số mìn xung quanh sau lần mở đầu tiên
    private void PlaceMines(int firstX, int firstY)
    {
        var valid = new List<(int X, int Y)>();
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (Math.Abs(x - firstX) > 1 || Math.Abs(y - firstY) > 1)
                    valid.Add((x, y));

        for (int i = 0; i < MineCount; i++)
        {
            int j = Random.Shared.Next(i, valid.Count);
            (valid[i], valid[j]) = (valid[j], valid[i]);
            _cells[valid[i].X, valid[i].Y].IsMine = true;
        }

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (!_cells[x, y].IsMine)
                    _cells[x, y].AdjacentMines = GetNeighbors(x, y).Count(n => _cells[n.X, n.Y].IsMine);
        
        IsStarted = true;
    }

    // Đổi trạng thái ô thành Revealed, thêm vào changed và queue
    private void Reveal(int x, int y, List<(int X, int Y)> changed, Queue<(int X, int Y)> queue)
    {
        _cells[x, y].State = CellState.Revealed;
        changed.Add((x, y));
        queue.Enqueue((x, y));
    }

    // Mở lan cho các ô an toàn, trả về true nếu trúng mìn
    private bool ProcessRevealQueue(Queue<(int X, int Y)> queue, List<(int X, int Y)> changed)
    {
        while (queue.Count > 0)
        {
            var (cx, cy) = queue.Dequeue();
            var cell = _cells[cx, cy];

            if (cell.IsMine)
            {
                LastHit = (cx, cy);
                return true;
            }

            _revealedCount++;
            if (cell.AdjacentMines == 0)
            {
                foreach (var (nx, ny) in GetNeighbors(cx, cy))
                {
                    var nCell = _cells[nx, ny];
                    if (nCell.State != CellState.Revealed && nCell.State != CellState.Flagged)
                    {
                        Reveal(nx, ny, changed, queue);
                    }
                }
            }
        }
        return false;
    }

    // Kiểm tra điều kiện thắng và tự động cắm cờ các mìn còn lại
    private void CheckWinCondition(List<(int X, int Y)> changed)
    {
        if (_revealedCount == _totalSafeCells)
        {
            IsOver = true;
            IsWin = true;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var c = _cells[x, y];
                    if (c.IsMine && c.State != CellState.Flagged)
                    {
                        c.State = CellState.Flagged;
                        _flaggedCount++;
                        changed.Add((x, y));
                    }
                }
            }
        }
    }

    // Kết thúc logic mở ô, bắn event tương ứng với kết quả
    private void FinalizeReveal(bool hitMine, bool isFirstOpen, List<(int X, int Y)> changed)
    {
        if (isFirstOpen) GameStarted?.Invoke();

        if (hitMine)
        {
            IsOver = true;
            IsWin = false;
            GameLost?.Invoke();
        }
        else
        {
            CheckWinCondition(changed);
            if (IsWin) GameWon?.Invoke();
        }
    }

    // Mở ô, mở lan nếu ô trống, trả về các ô thay đổi
    public List<(int X, int Y)> OpenCell(int x, int y)
    {
        var changed = new List<(int X, int Y)>();
        if (!IsValid(x, y) || IsOver || _cells[x, y].State == CellState.Flagged || _cells[x, y].State == CellState.Revealed)
            return changed;

        bool isFirstOpen = !IsStarted;
        if (isFirstOpen) PlaceMines(x, y);

        var queue = new Queue<(int X, int Y)>();
        Reveal(x, y, changed, queue);
        bool hitMine = ProcessRevealQueue(queue, changed);
        
        FinalizeReveal(hitMine, isFirstOpen, changed);
        return changed;
    }

    // Chuyển trạng thái cờ giữa Hidden, Flagged, Questioned
    public List<(int X, int Y)> ToggleMark(int x, int y)
    {
        var changed = new List<(int X, int Y)>();
        if (!IsValid(x, y) || IsOver || _cells[x, y].State == CellState.Revealed) return changed;

        var cell = _cells[x, y];
        if (cell.State == CellState.Hidden)
        {
            cell.State = CellState.Flagged;
            _flaggedCount++;
        }
        else if (cell.State == CellState.Flagged)
        {
            cell.State = CellState.Questioned;
            _flaggedCount--;
        }
        else
        {
            cell.State = CellState.Hidden;
        }
        changed.Add((x, y));
        return changed;
    }

    // Mở nhanh xung quanh nếu số cờ khớp với số mìn lân cận
    public List<(int X, int Y)> Chord(int x, int y)
    {
        var changed = new List<(int X, int Y)>();
        if (!IsStarted || IsOver || !IsValid(x, y)) return changed;
        
        var cell = _cells[x, y];
        if (cell.State != CellState.Revealed || cell.AdjacentMines == 0) return changed;
        if (GetNeighbors(x, y).Count(n => _cells[n.X, n.Y].State == CellState.Flagged) != cell.AdjacentMines)
            return changed;

        bool hitMine = false;
        foreach (var (nx, ny) in GetNeighbors(x, y))
        {
            if (_cells[nx, ny].State != CellState.Revealed && _cells[nx, ny].State != CellState.Flagged)
            {
                var queue = new Queue<(int X, int Y)>();
                Reveal(nx, ny, changed, queue);
                hitMine = ProcessRevealQueue(queue, changed);
                if (hitMine) break;
            }
        }
        
        if (changed.Count > 0) FinalizeReveal(hitMine, false, changed);
        return changed;
    }

    // Lấy các ô mìn chưa mở hoặc chưa cắm cờ để hiện khi thua
    public List<(int X, int Y)> GetMinesToExplode()
    {
        var list = new List<(int X, int Y)>();
        if (!IsOver || IsWin) return list;

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (_cells[x, y].IsMine && _cells[x, y].State != CellState.Flagged && _cells[x, y].State != CellState.Revealed)
                    list.Add((x, y));
            
        list.Sort((a, b) => 
        {
            int da = (a.X - LastHit.X) * (a.X - LastHit.X) + (a.Y - LastHit.Y) * (a.Y - LastHit.Y);
            int db = (b.X - LastHit.X) * (b.X - LastHit.X) + (b.Y - LastHit.Y) * (b.Y - LastHit.Y);
            return da.CompareTo(db);
        });
        return list;
    }

    // Lấy các ô cắm cờ nhưng không có mìn
    public List<(int X, int Y)> GetWrongFlags()
    {
        var list = new List<(int X, int Y)>();
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (_cells[x, y].State == CellState.Flagged && !_cells[x, y].IsMine)
                    list.Add((x, y));
        return list;
    }
}
