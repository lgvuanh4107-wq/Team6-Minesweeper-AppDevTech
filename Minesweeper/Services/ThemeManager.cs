using System;
using System.Linq;
using System.Windows;

namespace Minesweeper.Services;

public static class ThemeManager
{
    public static bool IsDark { get; private set; }

    // Đổi theme qua lại giữa sáng và tối
    public static void Toggle()
    {
        IsDark = !IsDark;
        var dicts = Application.Current.Resources.MergedDictionaries;
        
        var themeDict = dicts.FirstOrDefault(d => 
            d.Source != null && 
            (d.Source.OriginalString.EndsWith("LightTheme.xaml") || d.Source.OriginalString.EndsWith("DarkTheme.xaml")));

        if (themeDict != null)
        {
            dicts.Remove(themeDict);
        }

        dicts.Add(new ResourceDictionary 
        { 
            Source = new Uri(IsDark ? "Resources/DarkTheme.xaml" : "Resources/LightTheme.xaml", UriKind.Relative) 
        });
    }
}
