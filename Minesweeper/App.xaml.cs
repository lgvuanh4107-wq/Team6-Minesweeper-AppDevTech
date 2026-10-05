using System.Windows;
using Minesweeper.Models;
using Minesweeper.Views;

namespace Minesweeper;

public partial class App : Application
{
    // Bắt đầu chạy ứng dụng
    protected override void OnStartup(StartupEventArgs e)
    {
        new LeaderboardWindow("Dễ").ShowDialog();
    }
}
