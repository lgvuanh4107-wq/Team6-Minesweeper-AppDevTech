using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Minesweeper.Models;
using Minesweeper.Services;

namespace Minesweeper.Views
{
    public partial class MainWindow : Window
    {
        private readonly GameSession _session;
        private readonly Board _board;
        private Button[,] _buttons = null!;
        private bool _endShown;

        private readonly record struct Pos(int X, int Y);

        public MainWindow(GameSession session)
        {
            InitializeComponent();
            _session = session;
            _board = new Board(session.Difficulty);

            TitleTextBlock.Text = $"Độ khó: {session.Difficulty.Name} {session.Difficulty.Width}x{session.Difficulty.Height} • {session.PlayerName}";

            InitializeBoardLayout();
            UpdateStats();
        }

        private void InitializeBoardLayout()
        {
            int cols = _board.Width;
            int rows = _board.Height;
            double targetCellSize = 44; // Dễ: 44, Vừa: 36, Khó: 28

            if (_session.Difficulty == Difficulty.Medium) targetCellSize = 36;
            else if (_session.Difficulty == Difficulty.Hard) targetCellSize = 28;

            double screenWidth = SystemParameters.WorkArea.Width - 100;
            double screenHeight = SystemParameters.WorkArea.Height - 300;

            double maxCellWidth = screenWidth / cols;
            double maxCellHeight = screenHeight / rows;
            double actualCellSize = Math.Min(targetCellSize, Math.Min(maxCellWidth, maxCellHeight));

            BoardGrid.Columns = cols;
            BoardGrid.Rows = rows;
            BoardGrid.Width = actualCellSize * cols;
            BoardGrid.Height = actualCellSize * rows;

            _buttons = new Button[cols, rows];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    Button cellBtn = new Button
                    {
                        Width = actualCellSize,
                        Height = actualCellSize,
                        Focusable = false,
                        Padding = new Thickness(0),
                        FontSize = actualCellSize * 0.5,
                        Tag = new Pos(x, y)
                    };

                    cellBtn.SetResourceReference(Control.StyleProperty, "CellHidden");
                    cellBtn.Click += CellButton_Click;
                    cellBtn.MouseRightButtonUp += CellButton_RightClick;

                    _buttons[x, y] = cellBtn;
                    BoardGrid.Children.Add(cellBtn);
                }
            }
        }

        // Click trái: mở ô, hoặc chord nếu ô đã mở có số
        private void CellButton_Click(object sender, RoutedEventArgs e)
        {
            if (_board.IsOver) return;
            var pos = (Pos)((Button)sender).Tag;
            var cell = _board[pos.X, pos.Y];

            List<(int X, int Y)> changed;
            if (IsRevealed(cell))
            {
                if (cell.AdjacentMines <= 0) return;
                changed = _board.Chord(pos.X, pos.Y);
            }
            else
            {
                changed = _board.OpenCell(pos.X, pos.Y);
            }

            if (changed.Count > 0) PlaySfx("click");
            AfterMove(changed);
        }

        // Click phải: cắm cờ / dấu hỏi
        private void CellButton_RightClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (_board.IsOver) return;
            var pos = (Pos)((Button)sender).Tag;

            var changed = _board.ToggleMark(pos.X, pos.Y);
            if (changed.Count > 0) PlaySfx("flag");
            AfterMove(changed);
        }

        private void AfterMove(List<(int X, int Y)> changed)
        {
            foreach (var (x, y) in changed) RefreshCell(x, y);
            UpdateStats();

            if (_board.IsOver && !_endShown)
            {
                _endShown = true;
                if (_board.IsWin)
                {
                    PlaySfx("win");
                    // TODO (bước sau): hộp thoại thắng, lưu điểm
                }
                else
                {
                    ShowLoss();
                    PlaySfx("explode");
                    // TODO (bước sau): hộp thoại thua
                }
            }
        }

        private static bool IsRevealed(Cell cell) => cell.State == CellState.Revealed;
        private static bool IsFlagged(Cell cell) => cell.State == CellState.Flagged;
        private static bool IsQuestion(Cell cell) => cell.State == CellState.Questioned;

        private void RefreshCell(int x, int y)
        {
            var cell = _board[x, y];
            var btn = _buttons[x, y];

            if (IsRevealed(cell))
            {
                if (cell.IsMine)
                    Render(btn, true, "💣", "Brush.Mine", "Segoe UI Emoji");
                else if (cell.AdjacentMines > 0)
                    Render(btn, true, cell.AdjacentMines.ToString(), $"Brush.Number{cell.AdjacentMines}", null);
                else
                    Render(btn, true, null, null, null);
            }
            else if (IsFlagged(cell))
            {
                Render(btn, false, "\uE7C1", "Brush.Mine", "Segoe MDL2 Assets");
            }
            else if (IsQuestion(cell))
            {
                Render(btn, false, "?", "Brush.Question", null);
            }
            else
            {
                Render(btn, false, null, null, null);
            }
        }

        private static void Render(Button btn, bool revealed, string? text, string? foregroundKey, string? fontFamily)
        {
            btn.SetResourceReference(Control.StyleProperty, revealed ? "CellRevealed" : "CellHidden");
            btn.Content = text;

            if (fontFamily != null) btn.FontFamily = new FontFamily(fontFamily);
            else btn.ClearValue(Control.FontFamilyProperty);

            if (foregroundKey != null) btn.SetResourceReference(Control.ForegroundProperty, foregroundKey);
            else btn.ClearValue(Control.ForegroundProperty);
        }

        private void ShowLoss()
        {
            foreach (var (x, y) in _board.GetMinesToExplode())
                Render(_buttons[x, y], true, "💣", "Brush.Mine", "Segoe UI Emoji");

            foreach (var (x, y) in _board.GetWrongFlags())
                Render(_buttons[x, y], true, "✗", "Brush.Mine", null);

            var hit = _board.LastHit;
            _buttons[hit.X, hit.Y].SetResourceReference(Control.BackgroundProperty, "Brush.Mine");
        }

        private void UpdateStats()
        {
            MineCountTextBlock.Text = _board.FlagsRemaining.ToString();
            ProgressTextBlock.Text = $"Dọn bàn cờ {_board.ProgressPercent}%";
            GameProgressBarControl.Value = _board.ProgressPercent;
        }

        private static void PlaySfx(string name)
        {
            var s = name switch
            {
                "click" => Sound.Click,
                "flag" => Sound.Flag,
                "explode" => Sound.Explode,
                _ => Sound.Win
            };
            SoundManager.Play(s);
        }
    }
}