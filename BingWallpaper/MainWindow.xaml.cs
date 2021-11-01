using BingWallpaper.Extensions;
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

        private void LoadSucceed()
        {
            Ld_main.Dispatcher.Invoke(() => Ld_main.Visibility = Visibility.Hidden);
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

        private void Btn_switch_pre_Click(object sender, RoutedEventArgs e)
        {
            if (!Btn_switch_next.IsEnabled)
            {
                Btn_switch_next.IsEnabled = true;
            }

            var (previous, frontend) = _getter.Previous();

            if (previous is not null)
            {
                Img_bing.ChangeImageSource(previous.FilePath);
                Lb_copyright.ChangeContent(previous.Copyright);

                if (frontend)
                {
                    Btn_switch_pre.IsEnabled = false;
                }
            }
        }

        private void Btn_switch_next_Click(object sender, RoutedEventArgs e)
        {
            if (!Btn_switch_pre.IsEnabled)
            {
                Btn_switch_pre.IsEnabled = true;
            }

            var (next, backend) = _getter.Next();

            if (next is not null)
            {
                Img_bing.ChangeImageSource(next.FilePath);
                Lb_copyright.ChangeContent(next.Copyright);

                if (backend)
                {
                    Btn_switch_next.IsEnabled = false;
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

        private void OnDefaultLoaded(object? sender, EventArgs e)
        {
            var todayWallpaper = _getter.Default()!;

            Img_bing.ChangeImageSource(todayWallpaper.FilePath);
            Lb_copyright.ChangeContent(todayWallpaper.Copyright);
        }
    }
}
