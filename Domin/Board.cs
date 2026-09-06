using Domin.Domin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin
{
    public class Board
    {
        public int Width { get; }
        public int Height { get; }
        public Cell[,] Cells { get; }
        public int MineCount { get; }
        public bool IsGameOver { get; private set; }
        public bool IsWin { get; private set; }
        private int revealedCount;

        private static readonly int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        private static readonly int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        public Board(int width, int height, int mineCount)
          
        {
            Width = width;
            Height = height;
            MineCount = mineCount;
            Cells = new Cell[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    Cells[x, y] = new Cell();

            PlaceMines(mineCount);
            CalculateAdjacentMines();

        }
        public void OpenCell(int x, int y)
        {
            if (IsGameOver || !IsInsideBoard(x, y))
                return;

            Cell cell = Cells[x, y];

            if (cell.IsRevealed || cell.IsFlagged)
                return;

            cell.IsRevealed = true;
            revealedCount++;

            if (cell.IsMine)
            {
                IsGameOver = true;
                return;
            }

            CheckWin();

            if (cell.AdjacentMines == 0)
            {
                for (int i = 0; i < 8; i++)
                    OpenCell(x + dx[i], y + dy[i]);
            }
        }
        private void CheckWin()
        {
            if (revealedCount == Width * Height - MineCount)
            {
                IsGameOver = true;
                IsWin = true;
            }
        }

        private void PlaceMines(int mineCount)
        {
            var random = new Random();
            int placed = 0;

            while (placed < mineCount)
            {
                int x = random.Next(0, Width);
                int y = random.Next(0, Height);

                if (!Cells[x, y].IsMine)
                {
                    Cells[x, y].IsMine = true;
                    placed++;
                }
            }
        }
        private void CalculateAdjacentMines()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Cells[x, y].IsMine)
                        continue;

                    int count = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        int nx = x + dx[i];
                        int ny = y + dy[i];

                        if (IsInsideBoard(nx, ny) && Cells[nx, ny].IsMine)
                            count++;
                    }
                    Cells[x, y].AdjacentMines = count;
                }
            }
        }

        private bool IsInsideBoard(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }
    }
}
    

