using System;
using System.Windows;

namespace BingWallpaper.Utilities
{
    internal class LabelPositionGenerator
    {
        internal Thickness[] _margins;
        internal HorizontalAlignment[] _alignments;
        internal readonly Random _random = new(unchecked((int)DateTime.Now.Ticks));

        public LabelPositionGenerator()
        {
            _margins = new Thickness[2]
            {
                    new Thickness(0, 9, 0, 0),
                    new Thickness(0, 248, 0, 0)
            };

            _alignments = new HorizontalAlignment[2] { HorizontalAlignment.Left, HorizontalAlignment.Right };
        }

        internal (Thickness Thickness, HorizontalAlignment Alignment) Random()
            => (_margins[_random.Next(0, 2)], _alignments[_random.Next(0, 2)]);
    }
}
