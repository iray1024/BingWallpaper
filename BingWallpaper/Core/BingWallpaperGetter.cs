using BingWallpaper.Common;
using BingWallpaper.Core.Abstractions;
using BingWallpaper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace BingWallpaper.Core
{
    public class BingWallpaperGetter : IBingWallpaperGetter
    {
        private static string BING_WALLPAPER_API_URL = "https://cn.bing.com/HPImageArchive.aspx?format=js&n=8&idx=0";
        private const string BING_WALLPAPER_URL_PREFIX = "https://cn.bing.com";

        private readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(3) };
        private BingWallpaperOperator _operator = new();

        private bool _prepared = false;
        private bool _reloaded = false;
        private readonly string _savePath;

        public static BingWallpaperGetter Instance { get; } = new();

        private BingWallpaperGetter()
        {
            _savePath = Path.Combine(Path.GetTempPath(), ".wallpaper");

            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }
        }

        public string GetBingWallpaperAPIEndpoint()
            => BING_WALLPAPER_API_URL;

        public bool SetBingWallpaperAPIEndpoint(string endpoint)
        {
            if (VerifyBingWallpaperAPIEndpoint(endpoint))
            {
                BING_WALLPAPER_API_URL = endpoint;

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool VerifyBingWallpaperAPIEndpoint(string endpoint)
        {
            try
            {
                var @operator = BingWallpaperOperatorFactory.Operator(endpoint).Result;

                if (@operator is null || @operator.Images.Count == 0)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public BingWallpaperAggregation Previous()
            => new(_operator.Previous(), _operator.GetIndex(), _operator.GetIndexState());

        public BingWallpaperAggregation Next()
            => new(_operator.Next(), _operator.GetIndex(), _operator.GetIndexState());

        public BingWallpaperAggregation Default()
            => new(_operator.Default(), _operator.GetIndex(), _operator.GetIndexState());

        public BingWallpaperAggregation Current()
            => new(_operator.Current(), _operator.GetIndex(), _operator.GetIndexState());

        public void Initialize()
        {
            _prepared = false;

            Task.Run(async () =>
            {
                _operator = await BingWallpaperOperatorFactory.Operator(BING_WALLPAPER_API_URL) ?? _operator;

                var fault_tolerance = _operator.Images.Count;

                foreach (var item in _operator.Images)
                {
                    item.Copyright = item.Copyright.Split(" (")[0];
                    var filePath = Path.Combine(_savePath, $"bing_{item.EndDate}_{item.Copyright}.jpg");
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
                        await _httpClient.DownloadFileAsync($"{BING_WALLPAPER_URL_PREFIX}{item.Url}", filePath);
                        if (!_prepared)
                        {
                            _prepared = true;
                        }
                    }
                    catch (HttpRequestException)
                    {
                        fault_tolerance--;
                    }
                }

                if (fault_tolerance == 0)
                {
                    MessageBox.Show("网络连接失败", "发生错误");

                    Environment.Exit(-1);
                }

                LoadRemainWallpaper();

            }).Wait();
        }

        public void Reload(int breakpoint)
        {
            _reloaded = false;

            Task.Run(async () =>
            {
                var temp = await BingWallpaperOperatorFactory.Operator(BING_WALLPAPER_API_URL) ?? new();

                var fault_tolerance = temp.Images.Count;

                var index = 0;

                foreach (var item in temp.Images)
                {
                    item.Copyright = item.Copyright.Split(" (")[0];
                    var filePath = Path.Combine(_savePath, $"bing_{item.EndDate}_{item.Copyright}.jpg");
                    item.FilePath = filePath;

                    if (File.Exists(filePath))
                    {
                        index++;
                        continue;
                    }

                    try
                    {
                        await _httpClient.DownloadFileAsync($"{BING_WALLPAPER_URL_PREFIX}{item.Url}", filePath);
                    }
                    catch (HttpRequestException)
                    {
                        fault_tolerance--;
                    }

                    if (index == breakpoint)
                    {
                        _reloaded = true;
                    }

                    index++;
                }

                if (fault_tolerance == 0)
                {
                    MessageBox.Show("网络连接失败", "发生错误");

                    Environment.Exit(-1);
                }

                LoadRemainWallpaper();

            }).Wait();
        }

        private void LoadRemainWallpaper()
        {
            var tempList = new List<BingWallpaperObject>();

            foreach (var filepath in Directory.EnumerateFiles(_savePath))
            {
                if (!Path.GetExtension(filepath).Equals(".jpg"))
                {
                    continue;
                }

                if (_operator.Images.Any(n => n.FilePath.Equals(filepath)))
                {
                    continue;
                }

                var fileName = Path.GetFileNameWithoutExtension(filepath);

                var tempData = fileName.Split("_");

                var wallpapaer = new BingWallpaperObject()
                {
                    FilePath = filepath,
                    EndDate = tempData[1],
                    Copyright = tempData[2]
                };

                tempList.Add(wallpapaer);
            }

            (_operator.Images as List<BingWallpaperObject>)?.AddRange(tempList.OrderByDescending(n => n.EndDate));
        }

        public bool Prepared()
            => _prepared;

        public bool Reloaded()
            => _reloaded;
    }
}
