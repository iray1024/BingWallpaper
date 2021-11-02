using BingWallpaper.Models;
using System.Collections.Generic;

namespace BingWallpaper.Utilities
{
    internal class BingWallpaperOperator
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

        public BingWallpaperObject? Indexof(int index)
        {
            if (index < 0 || index >= Images.Count)
            {
                return null;
            }

            _index = index;

            return Images[index];
        }

        public IndexState GetIndexState()
        {
            if (_index == 0)
            {
                return IndexState.FrontEnd;
            }
            else if (_index == Images.Count - 1)
            {
                return IndexState.BackEnd;
            }

            return IndexState.Normal;
        }

        public int GetIndex()
            => _index;
    }
}
