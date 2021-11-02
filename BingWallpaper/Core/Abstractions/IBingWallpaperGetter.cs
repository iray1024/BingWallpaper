using BingWallpaper.Models;

namespace BingWallpaper.Core.Abstractions
{
    internal interface IBingWallpaperGetter
    {
        void Initialize();

        void Reload(int breakpoint);

        BingWallpaperAggregation Previous();

        BingWallpaperAggregation Next();

        BingWallpaperObject? Default();

        BingWallpaperObject? Current();

        bool Prepared();

        bool Reloaded();

        string GetBingWallpaperAPIEndpoint();

        bool SetBingWallpaperAPIEndpoint(string endpoint);

        bool VerifyBingWallpaperAPIEndpoint(string endpoint);
    }
}
