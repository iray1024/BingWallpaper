﻿using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BingWallpaper.Extensions
{
    internal static class HttpClientExtensions
    {
        internal static async Task DownloadFileAsync(this HttpClient client, string address, string filePath)
        {
            using var response = await client.GetAsync(address) ?? throw new HttpRequestException();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(filePath, FileMode.CreateNew);

            var buffer = new byte[1024];

            while (await stream.ReadAsync(buffer) != 0)
            {
                fileStream.Write(buffer);
            }

            stream.Close();
            fileStream.Flush();
            fileStream.Close();
        }
    }
}
