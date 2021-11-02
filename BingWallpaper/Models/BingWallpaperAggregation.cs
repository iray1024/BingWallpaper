using BingWallpaper.Core;

namespace BingWallpaper.Models
{
    public class BingWallpaperAggregation
    {
        public BingWallpaperObject? Instance { get; set; }

        public int Index { get; set; }

        public IndexState IndexState { get; set; }

        public BingWallpaperAggregation(BingWallpaperObject? bingWallpaperObject, int index, IndexState indexState = IndexState.Normal)
        {
            Instance = bingWallpaperObject;
            Index = index;
            IndexState = indexState;
        }
    }
}
