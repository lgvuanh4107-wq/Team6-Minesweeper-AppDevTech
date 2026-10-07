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

    private void BtnPlay_Click(object sender, RoutedEventArgs e)
    {
        string rawName = TxtPlayerName.Text.Trim();
        string playerName = string.IsNullOrEmpty(rawName) ? "Khách" : rawName;
        Difficulty difficulty = GetSelectedDifficulty();

        var session = new GameSession(playerName, difficulty);
        var mainWindow = new MainWindow(session);
        mainWindow.Show();
        this.Close();
    }

    private void BtnExit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void BtnTheme_Click(object sender, RoutedEventArgs e)
    {
        ThemeManager.Toggle();
            }
}