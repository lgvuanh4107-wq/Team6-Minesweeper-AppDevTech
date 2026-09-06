using Domin.Domin;
using System.Runtime.CompilerServices;

namespace Domin
{
    public partial class Form1 : Form
    {
        private const int Width = 9;
        private const int Height = 9;
        private const int MineCount = 10;
        private const int CellSize = 30;

        private Board board;
        private Button[,] buttons;

        public Form1()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void StartNewGame()
        {
            board = new Board(Width, Height, MineCount);
            buttons = new Button[Width, Height];

            Controls.Clear();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Button btn = new Button
                    {
                        Width = CellSize,
                        Height = CellSize,
                        Left = x * CellSize,
                        Top = y * CellSize,
                        Tag = (x, y)
                    };
                    btn.MouseUp += Cell_MouseUp;

                    buttons[x, y] = btn;
                    Controls.Add(btn);
                }
            }
        }

        private void Cell_MouseUp(object sender, MouseEventArgs e)
        {
            var (x, y) = ((int, int))((Button)sender).Tag;

            if (e.Button == MouseButtons.Left)
                board.OpenCell(x, y);
            else if (e.Button == MouseButtons.Right)
                board.ToggleFlag(x, y);

            RefreshUI();

            if (board.IsGameOver)
                MessageBox.Show(board.IsWin ? "You win!" : "Game over!");
        }

        private void RefreshUI()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Cell cell = board.Cells[x, y];
                    Button btn = buttons[x, y];

                    if (cell.IsRevealed)
                    {
                        btn.Text = cell.IsMine ? "*" : (cell.AdjacentMines == 0 ? "" : cell.AdjacentMines.ToString());
                        btn.Enabled = false;
                    }
                    else
                    {
                        btn.Text = cell.IsFlagged ? "F" : "";
                    }
                }
            }
        }
    }
}

    

