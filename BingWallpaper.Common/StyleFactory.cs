using System.Windows;
using System.Windows.Controls;

namespace BingWallpaper.Common
{
    public class StyleFactory
    {
#pragma warning disable CS8618
        public static BingStyle Normal { get; set; }
        public static BingStyle FullScreen { get; set; }
#pragma warning restore CS8618

        public static void Initialize(DependencyObject window, DependencyObject videoView)
        {
            var wnd = window as Window;
            var view = videoView as Control;

            Normal = new BingStyle(wnd!, view!);

            FullScreen = new BingStyle(BingWindowDetails.FullScreen(), BingControlDetails.FullScreen());
        }
    }

    internal static class BingStyleExtensions
    {
        public static void MapToWindowDetails(this BingWindowDetails style, Window @object)
        {
            style.Left = @object.Left;
            style.Top = @object.Top;
            style.Width = @object.Width;
            style.Height = @object.Height;
            style.WindowStyle = @object.WindowStyle;
        }

        public static void MapToVideoViewDetails(this BingControlDetails style, Control @object)
        {
            style.Width = @object.Width;
            style.Height = @object.Height;
        }
    }
}