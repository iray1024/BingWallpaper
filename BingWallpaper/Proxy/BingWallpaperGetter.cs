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

        private BingWallpaperListOperator? _operator;

        public BingWallpaperGetter()
        {
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            Invoke().Wait();
        }

        public Uri Preview()
        {
            return new Uri(_operator?.Preview().Url!);
        }

        public Uri Next()
        {
            return new Uri(_operator?.Next().Url!);
        }

        public Uri Default()
        {
            return new Uri(_operator?.Default().Url!);
        }

        public string Current()
        {
            return _operator?.Current().Url!;
        }

        private async Task Invoke()
        {
            using var httpClient = new HttpClient();

            var stream = await httpClient.GetStreamAsync(BING_WALLPAPER_API_URL);

            _operator = await JsonSerializer.DeserializeAsync<BingWallpaperListOperator>(stream, _serializerOptions);

            var fault_tolerance = _operator?.Images.Count;

            foreach (var item in _operator!.Images)
            {
                var filePath = Path.Combine(Path.GetTempPath(), ".wallpaper", $"bing-{item.EndDate!}.jpg");
                item.FilePath = filePath;

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
        }
    }
}
