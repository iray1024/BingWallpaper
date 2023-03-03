using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BingWallpaper.Core;

internal static class BingWallpaperOperatorFactory
{
    private static readonly HttpClient HttpClient = new();
    private static readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };

    public static async Task<BingWallpaperOperator?> Operator(string apiUrl)
    {
        var stream = await HttpClient.GetStreamAsync(apiUrl).ConfigureAwait(false);

        return await JsonSerializer.DeserializeAsync<BingWallpaperOperator>(stream, _serializerOptions);
    }
}