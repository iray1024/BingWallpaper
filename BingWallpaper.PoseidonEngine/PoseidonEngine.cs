using BingWallpaper.Common;
using BingWallpaper.Common.Extensions;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace BingWallpaper.PoseidonEngine;

internal class PoseidonEngine
{
    private readonly LibVLC _libVLC;
    private readonly IVideoView _videoView;
    private readonly IntPtr _handle;

    private Media? _currentMedia;

    private PoseidonEngine()
    {
        _libVLC = _libVLC ?? throw new ArgumentNullException();
        _videoView = _videoView ?? throw new ArgumentNullException();
    }

    internal PoseidonEngine(IVideoView vv)
    {
        Core.Initialize();

        _libVLC = new LibVLC();

        _videoView = vv;
        _handle = ((HwndSource)PresentationSource.FromVisual((VideoView)_videoView)).Handle;

        _videoView.MediaPlayer = new MediaPlayer(_libVLC);
        _videoView.MediaPlayer.EndReached += OnEndReached;

        _videoView.MediaPlayer.Fullscreen = true;
        _videoView.MediaPlayer.AspectRatio = $"{StyleFactory.FullScreen.BingControlDetails.Width}:{StyleFactory.FullScreen.BingControlDetails.Height}";
    }

    public Media CreateMedia(string mrl, FromType type = FromType.FromPath, params string[] options)
    {
        return new Media(_libVLC, mrl, type, options);
    }

    public bool Play(Media media)
    {
        if (_videoView.MediaPlayer is null)
        {
            return false;
        }

        _currentMedia = media;

        return _videoView.MediaPlayer.Play(media);
    }

    private void OnEndReached(object? sender, EventArgs e)
    {
        var video = (VideoView)_videoView ?? throw new NullReferenceException();

        CrossThreadAccessor.Run(() =>
        {
            if (video.MediaPlayer is null)
            {
                return;
            }

            if (_currentMedia is not null)
            {
                ThreadPool.QueueUserWorkItem((state)
                    => video.CrossThreadAccess(() => video.MediaPlayer.Play(_currentMedia)));
            }
        });
    }

    public void SetDesktopWallpaper()
    {
        var video = (VideoView)_videoView ?? throw new NullReferenceException();

        CrossThreadAccessor.RunAsync(() =>
        {
            var wnd = Window.GetWindow(video);

            wnd.SetStyle(StyleFactory.FullScreen.BingWindowDetails);
            video.SetStyle(StyleFactory.FullScreen.BingControlDetails);

            var workerW = DesktopOperator.GetDesktopHideLayoutHandle();

            if (_videoView.MediaPlayer is null)
            {
                return;
            }

            var view = (VideoView)_videoView;

            DesktopOperator.SetDynamicWallpaper(_handle);
        });
    }
}