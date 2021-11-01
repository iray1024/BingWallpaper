using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace BingWallpaper.Utilities
{
    public class BingWallpaperGetter
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

        public BingWallpaperObject? Preview()
            => _operator.Preview();

        public BingWallpaperObject? Next()
            => _operator.Next();

        public BingWallpaperObject? Default()
            => _operator.Default();

        public BingWallpaperObject? Current()
            => _operator.Current();

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
                    item.Copyright = item.Copyright.Split(" (")[0];

                    var filePath = Path.Combine(_savePath, $"bing_{item.EndDate}.jpg");
                    item.FilePath = filePath;

                    if (File.Exists(filePath))
                    {
                        if (!_prepared)
                        {
                            _prepared = true;
                        }
                        continue;
                    }

                    try
                    {
                        await _client.DownloadFileTaskAsync($"{BING_WALLPAPER_URL_PREFIX}{item.Url}", filePath);
                        if (!_prepared)
                        {
                            _prepared = true;
                        }
                    }
                    catch (WebException)
                    {
                        fault_tolerance--;
                    }
                }

                if (fault_tolerance == 0)
                {
                    MessageBox.Show("网络连接失败", "发生错误");

                    Environment.Exit(-1);
                }
            }).Wait();
        }

        private bool _prepared = false;

        public bool Prepared()
        {
            return _prepared;
        }
    }
}
