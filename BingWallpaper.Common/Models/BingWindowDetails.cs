using System.Windows;

namespace BingWallpaper.Common
{
    public struct BingWindowDetails
    {
        public double Left { get; set; }

        public double Top { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public WindowStyle WindowStyle { get; set; }

        public static BingWindowDetails FullScreen()
        {
            return new BingWindowDetails
            {
                Left = 0,
                Top = 0,
                Width = SystemParameters.PrimaryScreenWidth,
                Height = SystemParameters.PrimaryScreenHeight,
                WindowStyle = WindowStyle.None
            };
        }
    }
}