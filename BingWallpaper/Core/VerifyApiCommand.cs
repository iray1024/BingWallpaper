using System.Windows.Input;

namespace BingWallpaper.Core
{
    internal class VerifyApiCommand
    {
        public static readonly RoutedUICommand OpenVerifyWndCommand =
            new("Open the window to verify the BingWallpaper API", "Verify", typeof(MainWindow));
    }
}