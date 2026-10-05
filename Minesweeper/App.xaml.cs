using System.Windows;
using Minesweeper.Services;
using Minesweeper.Models;
using Minesweeper.Views;

namespace Minesweeper;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        using (var db = new AppDbContext())
        {
            db.Database.EnsureCreated();
        }

        
        LeaderboardService.Initialize();

        new NameEntryWindow().Show();
    }
}