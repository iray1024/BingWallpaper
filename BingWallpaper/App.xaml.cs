﻿using BingWallpaper.Common;
using System.Windows;

namespace BingWallpaper;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        CrossThreadAccessor.Initialize();
    }
}