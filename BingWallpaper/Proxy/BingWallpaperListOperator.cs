using System.Collections.Generic;

namespace BingWallpaper.Proxy
{
    public class BingWallpaperListOperator
    {
        public List<BingWallpaperObject> Images { get; set; } = new();

        private int _index = 0;

        public BingWallpaperObject Default()
        {
            return Images[0];
        }

        public BingWallpaperObject Current()
        {
            return Images[_index];
        }

        public BingWallpaperObject Preview()
        {
            if (_index > 0)
            {
                return Images[_index--];
            }

            return Current();
        }

        public BingWallpaperObject Next()
        {
            if (_index < Images.Count - 1)
            {
                return Images[_index++];
            }

            return Current();
        }
    }

    public class BingWallpaperObject
    {
        public string? EndDate { get; set; }

        public string? Url { get; set; }

        public string? Copyright { get; set; }

        public string? FilePath { get; set; }
    }
}
