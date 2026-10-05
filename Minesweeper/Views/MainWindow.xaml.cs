using System.Windows;
using Minesweeper.Models;

namespace Minesweeper.Views;

public partial class MainWindow : Window
{
    private readonly GameSession _session;

    // Khởi tạo cửa sổ chính màn chơi với thông tin session truyền vào
    public MainWindow(GameSession session)
    {
        InitializeComponent();
        _session = session;
        TempText.Text = $"Màn game: {_session.PlayerName} - {_session.Difficulty.Name}";
    }
}