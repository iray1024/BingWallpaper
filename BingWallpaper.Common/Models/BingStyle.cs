using System.Windows;
using System.Windows.Controls;

namespace BingWallpaper.Common
{
    public class BingStyle
    {
        public BingWindowDetails BingWindowDetails { get; set; }

        public BingControlDetails BingControlDetails { get; set; }

        public BingStyle(Window window, Control videoView)
        {
            BingWindowDetails.MapToWindowDetails(window);
            BingControlDetails.MapToVideoViewDetails(videoView);
        }

        public BingStyle(BingWindowDetails windowDetails, BingControlDetails videoViewDetails)
        {
            BingWindowDetails = windowDetails;
            BingControlDetails = videoViewDetails;
        }
    }
}
