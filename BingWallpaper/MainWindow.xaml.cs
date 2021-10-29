using BingWallpaper.Proxy;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BingWallpaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
            var todayImgUri = _getter.Default();

            if (todayImgUri is not null)
            {
                Visibility = Btn_switch.Visibility = Visibility.Visible;

                Img_bing.Source = new BitmapImage(todayImgUri);
            }
        }

        //private static async Task<string> GetTodayBingWallpaperUrlAsync()
        //{
        //    using var client = new WebClient() { Encoding = Encoding.UTF8 };

        //    try
        //    {
        //        var html = await client.DownloadStringTaskAsync("https://cn.bing.com/");

        //        var match = Regex.Match(html, "rel=\"preload\".*?href=\"(.+?)\" as=\"image\"");

        //        return string.Format("https://cn.bing.com{0}", match.Groups[1].Value);
        //    }
        //    catch (WebException)
        //    {

        //        MessageBox.Show("获取必应壁纸失败", "BingWallpaper - Ray", MessageBoxButton.OK, MessageBoxImage.Stop);

        //        Environment.Exit(-1);

        //        return "";
        //    }
        //}

        //private static async Task<string> DownloadWallpaperFileAsync(string url)
        //{
        //    using var client = new WebClient();

        //    var filePath = Path.Combine(Path.GetTempPath(), ".wallpaper", $"bing-{DateTime.Now.Date:yyyy_mm_dd}.jpg");

        //    try
        //    {
        //        await client.DownloadFileTaskAsync(url, filePath);

        //        return filePath;
        //    }
        //    catch (WebException)
        //    {
        //        MessageBox.Show("下载必应壁纸失败");

        //        Environment.Exit(-1);

        //        return "";
        //    }
        //}

        private void Btn_switch_pre_Click(object sender, RoutedEventArgs e)
        {
            var preImgUri = _getter.Preview();

            if (preImgUri is not null)
            {
                Img_bing.Source = new BitmapImage(preImgUri);
            }
        }

        private void Btn_switch_next_Click(object sender, RoutedEventArgs e)
        {
            var nextImgUri = _getter.Next();

            if (nextImgUri is not null)
            {
                Img_bing.Source = new BitmapImage(nextImgUri);
            }
        }

        private void Btn_switch_Click(object sender, RoutedEventArgs e)
        {
            var pInvoke = SystemParametersInfo(20, 0, _getter.Current(), 2);

            if (pInvoke == 1)
            {
                Environment.Exit(pInvoke);
            }
        }
    }
}
