using System;

namespace BingWallpaper.Common
{
    public static class Verifier
    {
        public static void NotNull(object? @object)
        {
            if (@object is null)
            {
                throw new ArgumentNullException(nameof(@object));
            }
        }
    }
}