using System.Windows.Controls;

namespace BingWallpaper.Common.Extensions
{
    internal static class ControlExtensions
    {
        internal static void SetStyle(this Control control, BingControlDetails details)
        {
            control.Width = details.Width;
            control.Height = details.Height;
        }
    }
}