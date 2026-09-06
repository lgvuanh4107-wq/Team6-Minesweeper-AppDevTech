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
    }
}
