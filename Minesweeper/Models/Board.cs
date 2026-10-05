using System;
using System.Collections.Generic;

namespace Minesweeper.Models;

public class Board
{
    private readonly Cell[,] _cells;
    private readonly Difficulty _difficulty;

    public int Width => _difficulty.Width;
    public int Height => _difficulty.Height;
    public int MineCount => _difficulty.Mines;

    public Cell this[int x, int y] => _cells[x, y];

    public bool IsStarted { get; private set; } = false;
    public bool IsOver { get; private set; } = false;
    public bool IsWin { get; private set; } = false;

    public int FlagsRemaining => MineCount;
    public int ProgressPercent => 0;

    public (int X, int Y) LastHit { get; private set; }

    // Khởi tạo bảng dựa trên độ khó
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
    }

    // Mở ô tại vị trí (x, y)
    public List<(int X, int Y)> OpenCell(int x, int y)
    {
        return new List<(int X, int Y)>();
    }

    // Đánh dấu cờ hoặc bỏ đánh dấu
    public List<(int X, int Y)> ToggleMark(int x, int y)
    {
        return new List<(int X, int Y)>();
    }

    // Mở nhanh các ô xung quanh
    public List<(int X, int Y)> Chord(int x, int y)
    {
        return new List<(int X, int Y)>();
    }

    // Lấy các ô mìn để nổ
    public List<(int X, int Y)> GetMinesToExplode()
    {
        return new List<(int X, int Y)>();
    }

    // Lấy các ô cắm cờ sai
    public List<(int X, int Y)> GetWrongFlags()
    {
        return new List<(int X, int Y)>();
    }

    // bản khung, sẽ bỏ khi làm logic thật
#pragma warning disable CS0067
    public event Action? GameStarted;
    public event Action? GameWon;
    public event Action? GameLost;
#pragma warning restore CS0067
}
