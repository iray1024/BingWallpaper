using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BingWallpaper.Utilities
{
    public class Loading : Control
    {
        public static readonly DependencyProperty FillColorProperty;

        static Loading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Loading), new FrameworkPropertyMetadata(typeof(Loading)));

            FillColorProperty = DependencyProperty.Register(
                "FillColor",
                typeof(Color),
                typeof(Loading),
                new UIPropertyMetadata(Colors.DarkBlue, new PropertyChangedCallback(OnUriChanged)));
        }

        private static void OnUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        [Description("背景色"), Category("个性配置"), DefaultValue("#FF668899")]
        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }
    }
}
