using BingWallpaper.Models;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BingWallpaper.Utilities
{
    public class BingWallpaperGetter
    {
        private const string BING_WALLPAPER_API_URL = "https://cn.bing.com/HPImageArchive.aspx?format=js&n=8&mkt=zh-CN";
        private const string BING_WALLPAPER_URL_PREFIX = "https://cn.bing.com";

        private readonly WebClient _client = new() { Encoding = Encoding.UTF8 };
        private BingWallpaperOperator _operator = new();

        private bool _prepared = false;
        private bool _reloaded = false;
        private readonly string _savePath;

        public BingWallpaperGetter()
        {
            _savePath = Path.Combine(Path.GetTempPath(), ".wallpaper");

            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }
        }

        public BingWallpaperAggregation Previous()
            => new(_operator.Previous(), _operator.GetIndex(), _operator.GetIndexState());


        public BingWallpaperAggregation Next()
            => new(_operator.Next(), _operator.GetIndex(), _operator.GetIndexState());


        public BingWallpaperObject? Default()
            => _operator.Default();

        public BingWallpaperObject? Current()
            => _operator.Current();

        public void Initialize()
        {
            Task.Run(async () =>
            {
                _operator = await BingWallpaperOperatorFactory.Operator(BING_WALLPAPER_API_URL) ?? _operator;

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

        public void Reload(int index)
        {
            _reloaded = false;

            Task.Run(async () =>
            {
                var temp = await BingWallpaperOperatorFactory.Operator(BING_WALLPAPER_API_URL) ?? new();

                var fault_tolerance = temp.Images.Count;

                var idx = 0;

                foreach (var item in temp.Images)
                {
                    var filePath = Path.Combine(_savePath, $"bing_{item.EndDate}.jpg");

                    if (File.Exists(filePath))
                    {
                        idx++;
                        continue;
                    }

                    try
                    {
                        await _client.DownloadFileTaskAsync($"{BING_WALLPAPER_URL_PREFIX}{item.Url}", filePath);

                        if (idx == index)
                        {
                            _reloaded = true;
                        }
                    }
                    catch (WebException)
                    {
                        fault_tolerance--;
                    }

                    idx++;
                }

                if (fault_tolerance == 0)
                {
                    MessageBox.Show("网络连接失败", "发生错误");

                    Environment.Exit(-1);
                }
            }).Wait();
        }

        public bool Prepared()
            => _prepared;

        public bool Reloaded()
            => _reloaded;
    }
}
