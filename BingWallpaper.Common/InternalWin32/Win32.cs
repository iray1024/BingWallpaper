using System;
using System.Runtime.InteropServices;

namespace BingWallpaper.Common.InternalWin32
{
    internal class Win32
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", EntryPoint = "SendMessageTimeout", CharSet = CharSet.Unicode)]
        internal static extern int SendMessageTimeout(IntPtr hwnd, uint msg, UIntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags flag, uint timeout, out UIntPtr lpdwResult);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Unicode)]
        internal static extern bool ShowWindow(IntPtr hwnd, uint cmdShow);

        [DllImport("user32.dll", EntryPoint = "EnumWindows", CharSet = CharSet.Unicode)]
        internal static extern int EnumWindows(EnumWindowsProc hwnd, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo", CharSet = CharSet.Unicode)]
        internal static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("user32.dll", EntryPoint = "SetParent", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);


        internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    }
}
