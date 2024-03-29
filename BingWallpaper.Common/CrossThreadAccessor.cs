﻿using System;
using System.Windows.Threading;

namespace BingWallpaper.Common
{
    public static class CrossThreadAccessor
    {
        private static Action<Action, bool> _executor =
            (action, async) => action();

        public static void Initialize()
        {
            var dispatcher = Dispatcher.CurrentDispatcher;

            _executor = (action, async) =>
            {
                if (dispatcher.CheckAccess())
                {
                    action();
                }
                else
                {
                    if (async)
                    {
                        dispatcher.BeginInvoke(action);
                    }
                    else
                    {
                        dispatcher.Invoke(action);
                    }
                }
            };
        }

        public static void Run(Action action)
        {
            _executor(action, false);
        }

        public static void RunAsync(Action action)
        {
            _executor(action, true);
        }
    }
}
