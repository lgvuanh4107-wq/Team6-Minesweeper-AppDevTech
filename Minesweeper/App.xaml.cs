using System.Windows;
using Minesweeper.Views;

namespace Minesweeper;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        

        new NameEntryWindow().Show();
    }
}