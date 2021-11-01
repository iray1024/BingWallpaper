﻿using System.Collections.Generic;

namespace BingWallpaper.Utilities
{
    internal class BingWallpaperListOperator
    {
        public IList<BingWallpaperObject> Images { get; set; } = new List<BingWallpaperObject>();

        private int _index = 0;

        public BingWallpaperObject? Default()
            => Images.Count > 0 ? Images[0] : null;

        public BingWallpaperObject? Current()
            => Images.Count > 0 ? Images[_index] : null;

        public BingWallpaperObject? Previous()
            => _index > 0 ? Images[--_index] : null;

        public BingWallpaperObject? Next()
            => _index < Images.Count - 1 ? Images[++_index] : null;

        public bool FrontEnd()
            => _index == 0;

        public bool BackEnd()
            => _index == Images.Count - 1;
    }

    public class BingWallpaperObject
    {
        public string EndDate { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public string Copyright { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;
    }
}
