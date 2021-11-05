namespace BingWallpaper.PoseidonEngine.Server
{
    internal class ServerMessage
    {
        public ServerMessageType ServerMsgType { get; set; }

        public string? Value { get; set; }

        public int IntValue { get; set; }
    }

    public enum ServerMessageType
    {
        OpenUrl,
        InDeskTop,
        Volume
    }
}
