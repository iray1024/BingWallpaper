﻿using BingWallpaper.Extensions;
using BingWallpaper.Utilities;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

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
        }

        private void Wnd_main_Loaded(object sender, RoutedEventArgs e)
        {
            Setup();
        }

        private void Setup()
        {
            Task.Run(() => _getter.Initialize());

            Task.Run(() => Prepare());
        }

        private void Prepare()
        {
            while (true)
            {
                if (_getter.Prepared())
                {
                    var todayWallpaper = _getter.Default();

                    if (todayWallpaper is not null)
                    {
                        Ld_main.Dispatcher.Invoke(() => Ld_main.Visibility = Visibility.Hidden);
                        Img_bing.ChangeImageSource(todayWallpaper.FilePath);
                        Lb_copyright.ChangeContent(todayWallpaper.Copyright);
                    }
                    else
                    {
                        Lb_default.Dispatcher.Invoke(() => Lb_default.Visibility = Visibility.Visible);
                        Btn_switch_pre.IsEnabled = false;
                        Btn_switch.IsEnabled = false;
                        Btn_switch_next.IsEnabled = false;
                    }

                    return;
                }

                Task.Delay(10).Wait();
            }
        }

        private void Btn_switch_pre_Click(object sender, RoutedEventArgs e)
        {
            var preWallpaper = _getter.Preview();

            if (preWallpaper is not null)
            {
                Img_bing.ChangeImageSource(preWallpaper.FilePath);
                Lb_copyright.ChangeContent(preWallpaper.Copyright);
            }
        }

        private void Btn_switch_next_Click(object sender, RoutedEventArgs e)
        {
            var nextWallpaper = _getter.Next();

            if (nextWallpaper is not null)
            {
                Img_bing.ChangeImageSource(nextWallpaper.FilePath);
                Lb_copyright.ChangeContent(nextWallpaper.Copyright);
            }
        }

        private void Btn_switch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ = SystemParametersInfo(20, 0, _getter.Current()!.FilePath, 2);
            }
            catch
            {
                MessageBox.Show(this, "壁纸设置失败", "发生异常");
            }

        }

        private void OnDefaultLoaded(object? sender, EventArgs e)
        {
            var todayWallpaper = _getter.Default()!;

            Img_bing.ChangeImageSource(todayWallpaper.FilePath);
            Lb_copyright.ChangeContent(todayWallpaper.Copyright);
        }
    }
}
