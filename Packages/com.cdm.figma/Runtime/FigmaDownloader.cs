using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Cdm.Figma
{
    public class FigmaDownloader : IFigmaDownloader
    {
        public async Task<FigmaFile> DownloadFileAsync(string fileID, string personalAccessToken)
        {
            Debug.Log($"Downloading file: {fileID}");

            var fileContentJson = await FigmaApi.GetFileAsTextAsync(
                new FigmaFileRequest(personalAccessToken, fileID)
                {
                    geometry = "paths",
                    plugins = new[] { PluginData.Id }
                });

            var file = FigmaFile.FromString(fileContentJson);
            file.fileID = fileID;

            if (!string.IsNullOrEmpty(file.thumbnailUrl))
            {
                var thumbnail = await FigmaApi.GetThumbnailImageAsync(file.thumbnailUrl);
                if (thumbnail != null)
                {
                    file.thumbnail = Convert.ToBase64String(thumbnail);
                }
            }

            return file;
        }
    }
}