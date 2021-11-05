using BingWallpaper.Common.InternalWin32;
using System;

namespace BingWallpaper.Common
{
    public static class DesktopOperator
    {
        private static IntPtr _progman = Win32.FindWindow("Progman", "Program Manager");
        private static IntPtr _workerW = IntPtr.Zero;

        public static int SetStaticWallpaper(int uAction, int uParam, string lpvParam, int fuWinIni)
            => Win32.SystemParametersInfo(uAction, uParam, lpvParam, fuWinIni);

        public static IntPtr GetDesktopHideLayoutHandle()
        {
            return GetWorkerW();
        }

        private static IntPtr GetWorkerW()
        {
            _progman = Win32.FindWindow("Progman", "Program Manager");

            _ = Win32.SendMessageTimeout(_progman, 0x052c, UIntPtr.Zero, IntPtr.Zero, SendMessageTimeoutFlags.SMTO_NORMAL, 0x3e8, out _);

            _ = Win32.EnumWindows((topHandle, lParam) =>
            {
                var defView = Win32.FindWindowEx(topHandle, IntPtr.Zero, "SHELLDLL_DefView", string.Empty);

                if (defView != IntPtr.Zero)
                {
                    _workerW = Win32.FindWindowEx(IntPtr.Zero, topHandle, "WorkerW", string.Empty);

                    return false;
                }

                return true;
            }, IntPtr.Zero);

            return _progman;
        }

        public static IntPtr SetDynamicWallpaper(IntPtr hWndChild)
            => Win32.SetParent(hWndChild, _progman);
    }
}
