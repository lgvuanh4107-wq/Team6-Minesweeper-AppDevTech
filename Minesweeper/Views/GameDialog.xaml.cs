using System.Windows;
using System.Windows.Controls;

namespace Minesweeper.Views;

public partial class GameDialog : Window
{
    public int ResultIndex { get; private set; } = -1;

    // Khởi tạo GameDialog tĩnh, chỉ dùng nội bộ
    private GameDialog()
    {
        InitializeComponent();
    }

    // Hiển thị hộp thoại tuỳ chỉnh và trả về chỉ số của nút được nhấn
    public static int Show(Window owner, string title, string message, params string[] buttons)
    {
        var dialog = new GameDialog
        {
            Owner = owner
        };
        
        dialog.TitleText.Text = title;
        dialog.MessageText.Text = message;

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // capture loop variable
            var btn = new Button
            {
                Content = buttons[i],
                Style = (Style)dialog.FindResource(i == 0 ? "PrimaryButton" : "SecondaryButton"),
                Margin = new Thickness(8, 0, 8, 0),
                MinWidth = 80
            };
            btn.Click += (s, e) =>
            {
                dialog.ResultIndex = index;
                dialog.DialogResult = true;
                dialog.Close();
            };
            dialog.ButtonsGrid.Children.Add(btn);
        }

        dialog.ShowDialog();
        return dialog.ResultIndex;
    }
}
