using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BingWallpaper.PoseidonEngine.Extensions
{
    internal static class VideoViewExtensions
    {
        internal static PoseidonEngine Inject(this IVideoView vv)
        {
            return new PoseidonEngine(vv);
        }
    }
}
