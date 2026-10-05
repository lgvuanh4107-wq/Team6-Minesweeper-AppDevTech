using System.Windows;

namespace Minesweeper.Services
{

    public static class ThemeManager
    {
        private const string LightSource = "Resources/LightTheme.xaml";
        private const string DarkSource = "Resources/DarkTheme.xaml";

       
        public static bool IsDark { get; private set; }

       
        public static void Toggle()
        {
            var merged = Application.Current.Resources.MergedDictionaries;

            
            ResourceDictionary? current = null;
            foreach (var dict in merged)
            {
                string? source = dict.Source?.OriginalString;
                if (source != null && source.EndsWith("Theme.xaml"))
                {
                    current = dict;
                    break;
                }
            }

            IsDark = !IsDark;
            var next = new ResourceDictionary
            {
                Source = new Uri(IsDark ? DarkSource : LightSource, UriKind.Relative)
            };

            if (current != null)
            {
                int index = merged.IndexOf(current);
                merged[index] = next;
            }
            else
            {
                merged.Insert(0, next);
            }
        }
    }
}
