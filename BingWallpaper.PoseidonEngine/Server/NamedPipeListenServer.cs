using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace BingWallpaper.PoseidonEngine.Server
{
    internal class NamedPipeListenServer
    {
        private readonly string _pipeName;
        private readonly IList<NamedPipeServerStream> _serverPool = new List<NamedPipeServerStream>();
        public Action<ServerMessage, NamedPipeServerStream>? ProcessMessage;

        public NamedPipeListenServer(string pipeName)
        {
            _pipeName = pipeName;
        }

        protected NamedPipeServerStream Create()
        {
            var npss = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 10);

            _serverPool.Add(npss);
            
            return npss;
        }

        protected void Dispose(NamedPipeServerStream npss)
        {
            npss.Close();

            if (_serverPool.Contains(npss))
            {
                _serverPool.Remove(npss);
            }            
        }

        public void Run()
        {
            Task.Run(() =>
            {
                using var pipeServer = Create();

                pipeServer.WaitForConnection();
                
                var action = new Action(Run);

                action.BeginInvoke(null, null);

                try
                {
                    bool isRun = true;
                    while (isRun)
                    {
                        var str = "";
                        var strAll = "";

                        var sb = new StringBuilder();
                        var sr = new StreamReader(pipeServer);

                        while (pipeServer.CanRead && ((str = sr.ReadLine()) !=  null))
                        {
                            if (str == "#END")
                            {
                                strAll = sb.ToString();
                                if (strAll.EndsWith("\r\n\r\n"))
                                    break;
                            }
                            else
                            {
                                if (str == "")
                                    sb.AppendLine();
                                else
                                    sb.AppendLine(str);
                            }
                        }

                        strAll = strAll[..^"\r\n\r\n\r\n".Length];

                        ProcessMessage(JsonSerializer.Deserialize<ServerMessage>(strAll)!, pipeServer);

                        if (!pipeServer.IsConnected)
                        {
                            isRun = false;
                            break;
                        }

                        Thread.Sleep(50);
                    }
                }
                catch (IOException e)
                {
                    
                }
                finally
                {
                    Dispose(pipeServer);
                }
            });
        }        

        public void Stop()
        {
            for (int i = 0; i < _serverPool.Count; i++)
            {
                var item = _serverPool[i];

                Dispose(item);
            }
        }
    }
}
