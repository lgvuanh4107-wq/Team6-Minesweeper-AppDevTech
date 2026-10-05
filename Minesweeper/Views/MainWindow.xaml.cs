using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Minesweeper.Models;

namespace Minesweeper.Views
{
    public partial class MainWindow : Window
    {
        private GameSession _session;

        // Xóa hàm dựng không tham số hoặc để lại nếu WPF yêu cầu thiết kế, 
        // nhưng ta chỉ dùng hàm dựng có GameSession.
        public MainWindow(GameSession session)
        {
            InitializeComponent();
            _session = session;

            // Cập nhật text tiêu đề
            TitleTextBlock.Text = $"Độ khó: {session.Difficulty.Name} {session.Difficulty.Width}x{session.Difficulty.Height} • {session.PlayerName}";
            MineCountTextBlock.Text = session.Difficulty.Mines.ToString();

            InitializeBoardLayout();
        }

        // Thiết lập kích thước ô và tạo lưới các nút dựa trên độ khó tạm thời
        private void InitializeBoardLayout()
        {
            int cols = _session.Difficulty.Width;
            int rows = _session.Difficulty.Height;
            double targetCellSize = 44; // Dễ: 44, Vừa: 36, Khó: 28
            
            if (_session.Difficulty == Difficulty.Medium) targetCellSize = 36;
            else if (_session.Difficulty == Difficulty.Hard) targetCellSize = 28;

            // Tự động giảm kích thước nếu bàn cờ vượt quá màn hình
            double screenWidth = SystemParameters.WorkArea.Width - 100;
            double screenHeight = SystemParameters.WorkArea.Height - 300; // Trừ khoảng trống header
            
            double maxCellWidth = screenWidth / cols;
            double maxCellHeight = screenHeight / rows;
            double actualCellSize = Math.Min(targetCellSize, Math.Min(maxCellWidth, maxCellHeight));

            BoardGrid.Columns = cols;
            BoardGrid.Rows = rows;
            BoardGrid.Width = actualCellSize * cols;
            BoardGrid.Height = actualCellSize * rows;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Button cellBtn = new Button
                    {
                        Width = actualCellSize,
                        Height = actualCellSize,
                        Focusable = false,
                        Padding = new Thickness(0),
                        FontSize = actualCellSize * 0.5
                    };
                    
                    cellBtn.SetResourceReference(Button.StyleProperty, "CellHidden");
                    BoardGrid.Children.Add(cellBtn);
                }
            }

            ShowFakeCells(cols);
        }

        // Hiện tạm vài ô đã mở để kiểm tra bố cục (sẽ xoá ở Bước 6)
        private void ShowFakeCells(int cols)
        {
            if (BoardGrid.Children.Count > 12)
            {
                if (BoardGrid.Children[0] is Button btn1)
                {
                    btn1.SetResourceReference(Button.StyleProperty, "CellRevealed");
                    btn1.SetResourceReference(Button.ForegroundProperty, "Brush.Number1");
                    btn1.Content = "1";
                }
                if (BoardGrid.Children[1] is Button btn2)
                {
                    btn2.SetResourceReference(Button.StyleProperty, "CellRevealed");
                    btn2.SetResourceReference(Button.ForegroundProperty, "Brush.Number2");
                    btn2.Content = "2";
                }
                if (BoardGrid.Children[cols + 1] is Button btn3)
                {
                    btn3.SetResourceReference(Button.StyleProperty, "CellRevealed");
                    btn3.SetResourceReference(Button.ForegroundProperty, "Brush.Number3");
                    btn3.Content = "3";
                }
            }
        }
    }
}