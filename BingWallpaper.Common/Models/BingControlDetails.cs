using System.Windows;

namespace BingWallpaper.Common
{
    public struct BingControlDetails
    {
        public double Width { get; set; }

        public double Height { get; set; }

        public static BingControlDetails FullScreen()
        {
            return new BingControlDetails
            {
                Width = SystemParameters.PrimaryScreenWidth,
                Height = SystemParameters.PrimaryScreenHeight
            };
        }
    }
}
