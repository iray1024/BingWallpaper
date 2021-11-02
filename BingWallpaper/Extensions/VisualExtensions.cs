using System;
using System.Windows.Media;

namespace BingWallpaper.Extensions
{
    internal static class VisualExtensions
    {
        internal static void CrossThreadAccess(this Visual visual, Action action)
            => visual.Dispatcher.Invoke(action);

        internal static TResult CrossThreadAccess<TResult>(this Visual visual, Func<TResult> callback)
            => visual.Dispatcher.Invoke(callback);
    }
}
