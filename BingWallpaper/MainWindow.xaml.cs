using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        private string _imgPath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            Setup();
        }

        private async void Setup()
        {
            var url = await GetTodayBingWallpaperUrlAsync();

            _imgPath = await DownloadWallpaperFileAsync(url);

            if (!string.IsNullOrEmpty(_imgPath))
            {
                Visibility = Btn_switch.Visibility = Visibility.Visible;

                Img_bing.Source = new BitmapImage(new Uri(_imgPath));
            }
        }

        private static async Task<string> GetTodayBingWallpaperUrlAsync()
        {
            using var client = new WebClient() { Encoding = Encoding.UTF8 };

            var html = await client.DownloadStringTaskAsync("https://cn.bing.com/");

            var match = Regex.Match(html, "rel=\"preload\".*?href=\"(.+?)\" as=\"image\"");

            return string.Format("https://cn.bing.com{0}", match.Groups[1].Value);
        }

        private static async Task<string> DownloadWallpaperFileAsync(string url)
        {
            using var client = new WebClient();

            var filePath = Path.Combine(Path.GetTempPath(), ".wallpaper", $"bing-{DateTime.Now.Date:yyyy_mm_dd}.jpg");

            await client.DownloadFileTaskAsync(url, filePath);

            return filePath;

        }

        private void Btn_switch_Click(object sender, RoutedEventArgs e)
        {
            var pInvoke = SystemParametersInfo(20, 0, _imgPath, 2);

            if (pInvoke == 1)
            {
                Environment.Exit(pInvoke);
            }
        }
    }
}
