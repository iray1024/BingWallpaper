using System;
using System.Windows;

namespace BingWallpaper.Utilities
{
    public class ContentControlPositionGenerator
    {
        private readonly Thickness[] _margins;
        private readonly HorizontalAlignment[] _alignments;
        private readonly Random _random = new(unchecked((int)DateTime.Now.Ticks));

        public ContentControlPositionGenerator()
        {
            _margins = new Thickness[2]
            {
                new Thickness(0, 9, 0, 0),
                new Thickness(0, 248, 0, 0)
            };

            _alignments = new HorizontalAlignment[2] { HorizontalAlignment.Left, HorizontalAlignment.Right };
        }

        public (Thickness Thickness, HorizontalAlignment Alignment) Random()
            => (_margins[_random.Next(0, 2)], _alignments[_random.Next(0, 2)]);
    }
}
