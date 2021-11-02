using BingWallpaper.Extensions;
using BingWallpaper.Models;
using BingWallpaper.Utilities;
using System;
using System.IO;
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
            Btn_switch_next.IsEnabled = false;
            Btn_switch.IsEnabled = false;
            Btn_switch_pre.IsEnabled = false;

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
                        Img_bing.ChangeImageSource(todayWallpaper.FilePath);
                        Lb_copyright.ChangeContent(todayWallpaper.Copyright);

                        LoadSucceed();
                    }
                    else
                    {
                        LoadFailed();
                    }

                    return;
                }

                Task.Delay(10).Wait();
            }
        }

        private void Reload(Action action)
        {
            while (true)
            {
                if (_getter.Reloaded())
                {
                    action();
                    return;
                }

                Task.Delay(10).Wait();
            }
        }

        private void Btn_switch_pre_Click(object sender, RoutedEventArgs e)
        {
            if (!Btn_switch_next.IsEnabled)
            {
                Btn_switch_next.IsEnabled = true;
            }

            var previous = _getter.Previous();

            if (previous.Instance is not null)
            {
                if (CheckFileAvailable(previous.Instance.FilePath))
                {
                    Img_bing.ChangeImageSource(previous.Instance.FilePath);
                    Lb_copyright.ChangeContent(previous.Instance.Copyright);

                    if (previous.IndexState == IndexState.FrontEnd)
                    {
                        Btn_switch_pre.IsEnabled = false;
                    }
                }
                else
                {
                    Img_bing.Source = null;
                    var loading = new Loading();
                    Grd_main.Children.Add(loading);

                    Task.Run(() => _getter.Reload(previous.Index));
                    Task.Run(() => Reload(ReloadCallback(loading, previous)));
                }
            }
        }

        private void Btn_switch_next_Click(object sender, RoutedEventArgs e)
        {
            if (!Btn_switch_pre.IsEnabled)
            {
                Btn_switch_pre.IsEnabled = true;
            }

            var next = _getter.Next();

            if (next.Instance is not null)
            {
                if (CheckFileAvailable(next.Instance.FilePath))
                {
                    Img_bing.ChangeImageSource(next.Instance.FilePath);
                    Lb_copyright.ChangeContent(next.Instance.Copyright);

                    if (next.IndexState == IndexState.BackEnd)
                    {
                        Btn_switch_next.IsEnabled = false;
                    }
                }
                else
                {
                    Img_bing.Source = null;
                    var loading = new Loading();
                    Grd_main.Children.Add(loading);

                    Task.Run(() => _getter.Reload(next.Index));
                    Task.Run(() => Reload(ReloadCallback(loading, next)));
                }
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

        private void LoadSucceed()
        {
            Grd_main.Dispatcher.Invoke(() => Grd_main.Children.Remove(Ld_main));
            Btn_switch.Dispatcher.Invoke(() => Btn_switch.IsEnabled = true);
            Btn_switch_next.Dispatcher.Invoke(() => Btn_switch_next.IsEnabled = true);
        }

        private void LoadFailed()
        {
            Lb_default.Dispatcher.Invoke(() => Lb_default.Visibility = Visibility.Visible);
            Btn_switch_pre.Dispatcher.Invoke(() => Btn_switch_pre.IsEnabled = false);
            Btn_switch.Dispatcher.Invoke(() => Btn_switch.IsEnabled = false);
            Btn_switch_next.Dispatcher.Invoke(() => Btn_switch_next.IsEnabled = false);
        }

        private Action ReloadCallback(UIElement iElement, BingWallpaperAggregation bingWallpaper)
            => new(() =>
            {
                if (bingWallpaper.Instance is not null)
                {
                    Grd_main.Dispatcher.Invoke(() => Grd_main.Children.Remove(iElement));
                    Img_bing.Dispatcher.Invoke(() => Img_bing.ChangeImageSource(bingWallpaper.Instance.FilePath));
                    Lb_copyright.Dispatcher.Invoke(() => Lb_copyright.ChangeContent(bingWallpaper.Instance.Copyright));

                    if (bingWallpaper.IndexState is IndexState.FrontEnd)
                    {
                        Btn_switch_pre.Dispatcher.Invoke(() => Btn_switch_pre.IsEnabled = false);
                    }
                    else if (bingWallpaper.IndexState is IndexState.BackEnd)
                    {
                        Btn_switch_next.Dispatcher.Invoke(() => Btn_switch_next.IsEnabled = false);
                    }
                }
            });

        private static bool CheckFileAvailable(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
