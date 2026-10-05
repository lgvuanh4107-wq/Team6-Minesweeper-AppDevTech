using System.Windows;
using Minesweeper.ViewModels;

namespace Minesweeper.Views;

public partial class LeaderboardWindow : Window
{
    private readonly LeaderboardViewModel _viewModel;

    // Khởi tạo cửa sổ bảng xếp hạng
    public LeaderboardWindow(string difficulty, string? highlightName = null, int? highlightSeconds = null)
    {
        InitializeComponent();
        _viewModel = new LeaderboardViewModel(difficulty);
        DataContext = _viewModel;
    }

    // Tải điểm cho độ khó Dễ
    private void BtnEasy_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.LoadScores("Dễ");
    }

    // Tải điểm cho độ khó Vừa
    private void BtnMedium_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.LoadScores("Vừa");
    }

    // Tải điểm cho độ khó Khó
    private void BtnHard_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.LoadScores("Khó");
    }

    // Đóng cửa sổ
    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
