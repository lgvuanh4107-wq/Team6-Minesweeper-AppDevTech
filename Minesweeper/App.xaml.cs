using System.Windows;
using Minesweeper.Models;
using Minesweeper.Views;

namespace Minesweeper;

public partial class App : Application
{
    // Bắt đầu chạy ứng dụng và khởi tạo dữ liệu
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        using (var db = new AppDbContext())
        {
            db.Database.EnsureCreated();
        }

        var testSession = new GameSession("VuAnhTest", Difficulty.Easy, true);
        new MainWindow(testSession).Show();
    }
}
