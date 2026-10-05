using System.Windows;

namespace Minesweeper.Views;

public partial class LeaderboardWindow : Window
{
    // Khởi tạo bảng xếp hạng với độ khó tương ứng
    public LeaderboardWindow(string difficultyName, string? highlightName = null, int? highlightSeconds = null)
    {
        InitializeComponent();
        tbTitle.Text = $"Bảng xếp hạng: {difficultyName}";
    }
}
