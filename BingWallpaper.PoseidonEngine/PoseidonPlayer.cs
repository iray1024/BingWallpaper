using BingWallpaper.PoseidonEngine.Server;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using System.IO.Pipes;

namespace BingWallpaper.PoseidonEngine
{
    internal class PoseidonPlayer : VideoView
    {
        private readonly PoseidonEngine _poseidon;
        private readonly VideoView _videoView;

        public PoseidonPlayer(VideoView videoView)
        {
            _videoView = videoView;
            _poseidon = new PoseidonEngine(videoView);

            var server = new NamedPipeListenServer("PoseidonEngine") { ProcessMessage = ProcessMessage };

        }

        public static void ProcessMessage(ServerMessage msg, NamedPipeServerStream pipeServer)
        {
            switch (msg.ServerMsgType)
            {
                case ServerMessageType.OpenUrl:
                    {

                    }
                    break;
                case ServerMessageType.InDeskTop:
                    {

                    }
                    break;
                case ServerMessageType.Volume:
                    {

                    }
                    break;
            }

            pipeServer.Close();
        }
    }
}