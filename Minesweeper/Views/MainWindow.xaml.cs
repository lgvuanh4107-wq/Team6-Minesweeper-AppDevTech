using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
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

        private readonly Stopwatch _stopwatch = new();
        private readonly DispatcherTimer _timer;
        private bool _paused;

        // Số giây đã chơi, dùng cho bước sau khi gọi LeaderboardService.AddScore
        public int ElapsedSeconds => (int)_stopwatch.Elapsed.TotalSeconds;

        private readonly record struct Pos(int X, int Y);

        public MainWindow(GameSession session)
        {
            InitializeComponent();
            _session = session;
            _board = new Board(session.Difficulty);

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            _timer.Tick += (_, _) => UpdateTimerText();
            _board.GameStarted += OnGameStarted;
            Closed += (_, _) => _timer.Stop();

            TitleTextBlock.Text = $"Độ khó: {session.Difficulty.Name} {session.Difficulty.Width}x{session.Difficulty.Height} • {session.PlayerName}";

            InitializeBoardLayout();
            UpdateStats();
            UpdateTimerText();
            RestartButton.Tag = "Normal";
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

        // ===== Đồng hồ và Pause =====

        private void OnGameStarted()
        {
            _stopwatch.Restart();
            _timer.Start();
        }

        private void StopClock()
        {
            _stopwatch.Stop();
            _timer.Stop();
            UpdateTimerText();
        }

        private void UpdateTimerText()
        {
            var t = _stopwatch.Elapsed;
            TimerTextBlock.Text = $"{(int)t.TotalMinutes:00}:{t.Seconds:00}";
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_paused) ResumeGame();
            else PauseGame();
        }

        private void ResumeButton_Click(object sender, RoutedEventArgs e) => ResumeGame();

        private void PauseGame()
        {
            // Chỉ cho tạm dừng khi ván đang chạy
            if (!_board.IsStarted || _board.IsOver || _paused) return;

            _paused = true;
            _stopwatch.Stop();
            _timer.Stop();
            PauseOverlay.Visibility = Visibility.Visible;
            PauseButton.Content = "\uE768";
            PauseButton.ToolTip = "Tiếp tục";
        }

        private void ResumeGame()
        {
            if (!_paused) return;

            _paused = false;
            _stopwatch.Start();
            _timer.Start();
            PauseOverlay.Visibility = Visibility.Collapsed;
            PauseButton.Content = "\uE769";
            PauseButton.ToolTip = "Tạm dừng";
        }

        // ===== Thao tác trên bàn cờ =====

        // Click trái: mở ô, hoặc chord nếu ô đã mở có số
        private void CellButton_Click(object sender, RoutedEventArgs e)
        {
            if (_board.IsOver || _paused) return;
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
            if (_board.IsOver || _paused) return;
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
                StopClock();
                PauseButton.IsEnabled = false;

                if (_board.IsWin)
                {
                    PlaySfx("win");
                    HandleWin();
                }
                else
                {
                    _ = ShowLoseAnimationAsync();
                }
            }
        }

        // Hỏi xác nhận trước khi bỏ ván
        private bool ConfirmAbandon()
        {
            if (_board.IsStarted && !_board.IsOver)
            {
                bool wasPaused = _paused;
                if (!wasPaused) PauseGame();

                int res = GameDialog.Show(this, "Xác nhận", "Bạn có chắc chắn muốn bỏ ván hiện tại?", "Đồng ý", "Hủy");
                
                if (res == 0) return true;
                
                if (!wasPaused) ResumeGame();
                return false;
            }
            return true;
        }

        // Xử lý nút về menu
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmAbandon())
            {
                // Mở lại màn hình khởi động
                var entryWindow = new NameEntryWindow();
                Application.Current.MainWindow = entryWindow;
                entryWindow.Show();

                // Đóng cửa sổ chơi game hiện tại
                this.Close();
            }
        }

        // Xử lý nút chơi lại
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmAbandon())
            {
                new MainWindow(_session).Show();
                Close();
            }
        }

        // Xử lý mở bảng xếp hạng
        private void LeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            bool wasPaused = _paused;
            if (_board.IsStarted && !_board.IsOver && !wasPaused) PauseGame();

            new LeaderboardWindow(_session.Difficulty.Name) { Owner = this }.ShowDialog();

            if (_board.IsStarted && !_board.IsOver && !wasPaused) ResumeGame();
        }

        // Xử lý chiến thắng và lưu điểm
        private void HandleWin()
        {
            RestartButton.Tag = "Win";
            string rankMsg = "";
            if (_session.SaveScore)
            {
                int? rank = LeaderboardService.AddScore(_session.PlayerName, _session.Difficulty.Name, ElapsedSeconds);
                if (rank.HasValue) rankMsg = $"\nThứ hạng: {rank.Value}";
            }

            int res = GameDialog.Show(this, "Chiến thắng!", $"Thời gian: {ElapsedSeconds} giây{rankMsg}", "Chơi lại", "Xếp hạng", "Menu");
            if (res == 0)
            {
                new MainWindow(_session).Show();
                Close();
            }
            else if (res == 1)
            {
                new LeaderboardWindow(_session.Difficulty.Name) { Owner = this }.ShowDialog();
            }
            else if (res == 2)
            {
                Close();
            }
        }

        // Hiển thị hiệu ứng nổ mìn lan truyền khi thua
        private async System.Threading.Tasks.Task ShowLoseAnimationAsync()
        {
            RestartButton.Tag = "Lose";
            var hit = _board.LastHit;
            _buttons[hit.X, hit.Y].SetResourceReference(Control.BackgroundProperty, "Brush.Mine");
            PlaySfx("explode");

            foreach (var (x, y) in _board.GetMinesToExplode())
            {
                await System.Threading.Tasks.Task.Delay(50);
                Render(_buttons[x, y], true, "💣", "Brush.Mine", "Segoe UI Emoji");
            }

            foreach (var (x, y) in _board.GetWrongFlags())
            {
                Render(_buttons[x, y], true, "✗", "Brush.Mine", null);
            }

            int result = GameDialog.Show(this, "Thất bại", "Bạn đã đạp trúng mìn!", "Chơi lại", "Menu");
            if (result == 0)
            {
                new MainWindow(_session).Show();
                Close();
            }
            else if (result == 1)
            {
                Close();
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