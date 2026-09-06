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

        private static readonly int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        private static readonly int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        public Board(int width, int height, int mineCount)
        {
            Width = width;
            Height = height;
            Cells = new Cell[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    Cells[x, y] = new Cell();

            PlaceMines(mineCount);
            CalculateAdjacentMines();

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
    

