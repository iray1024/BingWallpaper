using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace BingWallpaper.Proxy
{
    internal class BingWallpaperGetter
    {
        private const string BING_WALLPAPER_API_URL = "https://cn.bing.com/HPImageArchive.aspx?format=js&n=8&mkt=zh-CN";
        private const string BING_WALLPAPER_URL_PREFIX = "https://cn.bing.com";

        private readonly WebClient _client = new() { Encoding = Encoding.UTF8 };
        private readonly JsonSerializerOptions _serializerOptions;

        private BingWallpaperListOperator _operator = new();

        private readonly string _savePath;

        public BingWallpaperGetter()
        {
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            _savePath = Path.Combine(Path.GetTempPath(), ".wallpaper");

            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }
        }

        public Uri? Preview()
        {
            var pre = _operator.Preview();

            return pre != null ? new Uri(pre.FilePath) : null;
        }

        public Uri? Next()
        {
            var next = _operator.Next();

            return next != null ? new Uri(next.FilePath) : null;
        }

        public Uri? Default()
        {
            var defaultImg = _operator.Default();

            return defaultImg != null ? new Uri(defaultImg.FilePath) : null;
        }

        public string Current()
        {
            var current = _operator.Current();

            return current != null ? current.FilePath : string.Empty;
        }

        public void Initialize()
        {
            Task.Run(async () =>
            {
                using var httpClient = new HttpClient();

                var stream = await httpClient.GetStreamAsync(BING_WALLPAPER_API_URL).ConfigureAwait(false);

                _operator = await JsonSerializer.DeserializeAsync<BingWallpaperListOperator>(stream, _serializerOptions) ?? _operator;

                var fault_tolerance = _operator.Images.Count;

                foreach (var item in _operator.Images)
                {
                    var filePath = Path.Combine(_savePath, $"bing_{item.EndDate}.jpg");
                    item.FilePath = filePath;

                    if (File.Exists(filePath))
                    {
                        continue;
                    }

                    try
                    {
                        await _client.DownloadFileTaskAsync($"{BING_WALLPAPER_URL_PREFIX}{item.Url}", filePath);
                    }
                    catch (WebException)
                    {
                        fault_tolerance--;
                    }
                }

                if (fault_tolerance == 0)
                {
                    MessageBox.Show("下载必应壁纸失败");

                    Environment.Exit(-1);
                }
            }).Wait();
        }
    }
}
