using BingWallpaper.Common;
using BingWallpaper.Core;
using BingWallpaper.Core.Abstractions;
using BingWallpaper.Extensions;
using BingWallpaper.Models;
using BingWallpaper.PoseidonEngine.Extensions;
using BingWallpaper.Utilities;
using LibVLCSharp.Shared;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BingWallpaper
{
    public partial class MainWindow : Window
    {
        private readonly IBingWallpaperGetter _getter = BingWallpaperGetter.Instance;

        public MainWindow()
        {
            InitializeComponent();

            StyleFactory.Initialize(this, Vlc);
        }

        private void Wnd_main_Loaded(object sender, RoutedEventArgs e)
        {
            UIElementInitialize();

            CoreInitialize();

            Vlc.Visibility = Visibility.Visible;

            var poseidon = Vlc.Inject();

            var media = poseidon.CreateMedia(@"C:\Users\appeon\Desktop\16049705148ab3f3be56f4b4d8.mp4_last.mp4", FromType.FromPath);

            poseidon.Play(media);

            poseidon.SetDesktopWallpaper();
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
                    var loading = ReadyToReload();

                    Task.Run(() =>
                    {
                        _getter.Reload(previous.Index);
                        ReloadCompleted(previous);
                    });

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
                    var loading = ReadyToReload();

                    Task.Run(() =>
                    {
                        _getter.Reload(next.Index);
                        ReloadCompleted(next);
                    });

                    Task.Run(() => Reload(ReloadCallback(loading, next)));
                }
            }
        }

        private void Btn_switch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ = DesktopOperator.SetStaticWallpaper(20, 0, _getter.Current().Instance!.FilePath, 2);
            }
            catch
            {
                MessageBox.Show(this, "壁纸设置失败", "发生异常");
            }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var wnd = new VerifyWindow
            {
                Owner = this
            };

            var result = wnd.ShowDialog();

            if (result.HasValue && result.Value)
            {
                var loading = ReadyToReload();

                Task.Run(() =>
                {
                    _getter.Initialize();

                    var temp = _getter.Default();

                    ReloadCompleted(temp);
                });

                Task.Run(() => ReInitialize(ReloadCallback(loading, _getter.Default())));
            }
        }

        private void Lb_copyright_MouseEnter(object sender, MouseEventArgs e)
        {
            Lb_copyright.ChangeOpacoty(1, 0, TimeSpan.FromSeconds(0.4));
        }

        private void Lb_copyright_MouseLeave(object sender, MouseEventArgs e)
        {
            Lb_copyright.ChangeOpacoty(0, 1, TimeSpan.FromSeconds(0.4));
        }
    }

    public partial class MainWindow : Window
    {
        private void CoreInitialize()
        {
            Task.Run(() =>
            {
                _getter.Initialize();
                CrossThreadSwitchButtonState(ButtonState.FrontEnd);
            });

            Task.Run(() => Prepare());
        }

        private void Prepare()
        {
            var maxTime = 3000;
            while (true)
            {
                if (_getter.Prepared())
                {
                    var todayWallpaper = _getter.Default();

                    if (todayWallpaper is not null)
                    {
                        Img_bing.ChangeImageSource(todayWallpaper.Instance!.FilePath);
                        Lb_copyright.ChangeContent(todayWallpaper.Instance!.Copyright);

                        LoadSucceed();
                    }
                    else
                    {
                        LoadFailed();
                    }

                    return;
                }

                Task.Delay(10).Wait();
                maxTime -= 10;

                if (maxTime <= 0)
                {
                    LoadFailed();
                    break;
                }
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

        private void ReInitialize(Action action)
        {
            while (true)
            {
                if (_getter.Prepared())
                {
                    action();
                    return;
                }

                Task.Delay(10).Wait();
            }
        }

        private void UIElementInitialize()
        {
            UIThreadSwitchButtonState(ButtonState.Disable);
        }

        private void LoadSucceed()
        {
            Lb_copyright.CrossThreadAccess(() => Lb_copyright.Visibility = Visibility.Visible);
            Grd_main.CrossThreadAccess(() => Grd_main.Children.Remove(Ld_main));
        }

        private void LoadFailed()
        {
            Lb_default.Dispatcher.Invoke(() => Lb_default.Visibility = Visibility.Visible);
            CrossThreadSwitchButtonState(ButtonState.Disable);
        }

        private UIElement ReadyToReload()
        {
            CrossThreadSwitchButtonState(ButtonState.Disable);

            Img_bing.Source = null;

            var loading = Loading.Create();
            Grd_main.Children.Add(loading);

            return loading;
        }

        private void ReloadCompleted(BingWallpaperAggregation bingWallpaper)
        {
            if (bingWallpaper.IndexState is IndexState.FrontEnd)
            {
                CrossThreadSwitchButtonState(ButtonState.FrontEnd);
            }
            else if (bingWallpaper.IndexState is IndexState.BackEnd)
            {
                CrossThreadSwitchButtonState(ButtonState.BackEnd);
            }
            else if (bingWallpaper.IndexState is IndexState.Normal)
            {
                CrossThreadSwitchButtonState(ButtonState.Normal);
            }
        }

        private Action ReloadCallback(UIElement iElement, BingWallpaperAggregation bingWallpaper)
            => new(() =>
            {
                if (bingWallpaper.Instance is not null)
                {
                    Grd_main.CrossThreadAccess(() => Grd_main.Children.Remove(iElement));
                    Img_bing.ChangeImageSource(bingWallpaper.Instance.FilePath);
                    Lb_copyright.ChangeContent(bingWallpaper.Instance.Copyright);
                }
            });

        #region Utilities
        private static bool CheckFileAvailable(string filePath)
            => File.Exists(filePath);

        private void UIThreadSwitchButtonState(ButtonState state = ButtonState.Normal)
        {
            switch (state)
            {
                case ButtonState.Normal:
                    {
                        Btn_switch_pre.IsEnabled = true;
                        Btn_switch.IsEnabled = true;
                        Btn_switch_next.IsEnabled = true;
                    }
                    break;
                case ButtonState.FrontEnd:
                    {
                        Btn_switch_pre.IsEnabled = false;
                        Btn_switch.IsEnabled = true;
                        Btn_switch_next.IsEnabled = true;
                    }
                    break;
                case ButtonState.BackEnd:
                    {
                        Btn_switch_pre.IsEnabled = true;
                        Btn_switch.IsEnabled = true;
                        Btn_switch_next.IsEnabled = false;
                    }
                    break;
                case ButtonState.Disable:
                    {
                        Btn_switch_pre.IsEnabled = false;
                        Btn_switch.IsEnabled = false;
                        Btn_switch_next.IsEnabled = false;
                    }
                    break;
                default:
                    break;
            }
        }

        private void CrossThreadSwitchButtonState(ButtonState state = ButtonState.Normal)
        {
            switch (state)
            {
                case ButtonState.Normal:
                    {
                        Btn_switch_pre.CrossThreadAccess(() => Btn_switch_pre.IsEnabled = true);
                        Btn_switch.CrossThreadAccess(() => Btn_switch.IsEnabled = true);
                        Btn_switch_next.CrossThreadAccess(() => Btn_switch_next.IsEnabled = true);
                    }
                    break;
                case ButtonState.FrontEnd:
                    {
                        Btn_switch_pre.CrossThreadAccess(() => Btn_switch_pre.IsEnabled = false);
                        Btn_switch.CrossThreadAccess(() => Btn_switch.IsEnabled = true);
                        Btn_switch_next.CrossThreadAccess(() => Btn_switch_next.IsEnabled = true);
                    }
                    break;
                case ButtonState.BackEnd:
                    {
                        Btn_switch_pre.CrossThreadAccess(() => Btn_switch_pre.IsEnabled = true);
                        Btn_switch.CrossThreadAccess(() => Btn_switch.IsEnabled = true);
                        Btn_switch_next.CrossThreadAccess(() => Btn_switch_next.IsEnabled = false);
                    }
                    break;
                case ButtonState.Disable:
                    {
                        Btn_switch_pre.CrossThreadAccess(() => Btn_switch_pre.IsEnabled = false);
                        Btn_switch.CrossThreadAccess(() => Btn_switch.IsEnabled = false);
                        Btn_switch_next.CrossThreadAccess(() => Btn_switch_next.IsEnabled = false);
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}