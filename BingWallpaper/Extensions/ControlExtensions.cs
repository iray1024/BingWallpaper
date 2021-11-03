using BingWallpaper.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace BingWallpaper.Extensions
{
    internal static class ControlExtensions
    {
        private static readonly AnimationTimeline _opacityRaise = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.8));
        private static readonly ContentControlPositionGenerator _posGenerator = new();

        internal static void ChangeContent(this Label label, string content)
        {
            label.CrossThreadAccess(() =>
            {
                var position = _posGenerator.Random();

                label.Content = content;
                label.Opacity = 0D;

                label.Margin = position.Thickness;
                label.HorizontalContentAlignment = position.Alignment;

                label.BeginAnimation(UIElement.OpacityProperty, _opacityRaise, HandoffBehavior.SnapshotAndReplace);
            });
        }

        internal static void ChangeImageSource(this Image image, string path)
        {
            image.CrossThreadAccess(() =>
            {
                var bi = new BitmapImage();

                bi.BeginInit();
                bi.UriSource = new Uri(path);
                bi.DecodePixelWidth = 1920;
                bi.DecodePixelHeight = 1080;
                bi.CreateOptions = BitmapCreateOptions.DelayCreation;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();

                image.Opacity = 0D;
                image.Source = bi;

                image.BeginAnimation(UIElement.OpacityProperty, _opacityRaise, HandoffBehavior.SnapshotAndReplace);
            });
        }
    }
}