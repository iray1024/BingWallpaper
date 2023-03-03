namespace BingWallpaper.Abstractions
{
    public interface IRender
    {
        bool Render();

        void Restore();
    }
}