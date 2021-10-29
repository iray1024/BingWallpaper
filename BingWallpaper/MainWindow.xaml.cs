using BingWallpaper.Proxy;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BingWallpaper
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo", CharSet = CharSet.Unicode)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private readonly BingWallpaperGetter _getter = new();

        public MainWindow()
        {
            InitializeComponent();

            Setup();
        }

        private void Setup()
        {
            _getter.Initialize();

            var todayImgUri = _getter.Default();

            if (todayImgUri is not null)
            {
                Img_bing.Source = BitmapLoad(todayImgUri);
            }

            else
            {
                Lb_default.Visibility = Visibility.Visible;
                Btn_switch_pre.IsEnabled = false;
                Btn_switch.IsEnabled = false;
                Btn_switch_next.IsEnabled = false;
            }
        }
        private void Btn_switch_pre_Click(object sender, RoutedEventArgs e)
        {
            var preImgUri = _getter.Preview();

            if (preImgUri is not null)
            {
                Img_bing.Source = BitmapLoad(preImgUri);
            }
        }

        private void Btn_switch_next_Click(object sender, RoutedEventArgs e)
        {
            var nextImgUri = _getter.Next();

            if (nextImgUri is not null)
            {
                Img_bing.Source = BitmapLoad(nextImgUri);
            }
        }

        private void Btn_switch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ = SystemParametersInfo(20, 0, _getter.Current(), 2);
            }
            catch
            {
                MessageBox.Show(this, "壁纸设置失败", "发生异常");
            }

        }

        private static BitmapImage BitmapLoad(Uri uri)
        {
            var bi = new BitmapImage();

            bi.BeginInit();
            bi.UriSource = uri;
            bi.DecodePixelWidth = 1920;
            bi.EndInit();

            return bi;
        }
    }
}
