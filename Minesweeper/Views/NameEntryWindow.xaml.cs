using System.Windows;
using Minesweeper.Models;
using Minesweeper.Services;

namespace Minesweeper.Views;

public partial class NameEntryWindow : Window
{
public NameEntryWindow()
{
InitializeComponent();
}

private Difficulty GetSelectedDifficulty()
{
    if (RadMedium.IsChecked == true) return Difficulty.Medium;
    if (RadHard.IsChecked == true) return Difficulty.Hard;
    return Difficulty.Easy;
}

private void StartGame(bool saveScore)
{
    string rawName = TxtPlayerName.Text.Trim();

    if (saveScore && string.IsNullOrEmpty(rawName))
    {
        TxtError.Text = "Vui lòng nhập tên người chơi!";
        TxtError.Visibility = Visibility.Visible;
        return;
    }

    TxtError.Visibility = Visibility.Collapsed;
    string playerName = string.IsNullOrEmpty(rawName) ? "Khách" : rawName;
    Difficulty difficulty = GetSelectedDifficulty();

    var session = new GameSession(playerName, difficulty, saveScore);
    var mainWindow = new MainWindow(session);
    mainWindow.Show();
    this.Close();
}

private void BtnPlay_Click(object sender, RoutedEventArgs e)
{
    StartGame(saveScore: true);
}

private void BtnPlayNoSave_Click(object sender, RoutedEventArgs e)
{
    StartGame(saveScore: false);
}

private void BtnLeaderboard_Click(object sender, RoutedEventArgs e)
{
    // Difficulty diff = GetSelectedDifficulty();
    // var lbWindow = new LeaderboardWindow(diff.Name);
    // lbWindow.Owner = this;
    // lbWindow.ShowDialog();
    MessageBox.Show("Tính năng Bảng xếp hạng sử dụng Database đang được xây dựng và sẽ ra mắt ở giai đoạn sau!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
}

private void BtnSound_Click(object sender, RoutedEventArgs e)
{
}

private void BtnTheme_Click(object sender, RoutedEventArgs e)
{
    ThemeManager.Toggle();
}
}