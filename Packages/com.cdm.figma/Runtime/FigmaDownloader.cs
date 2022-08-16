using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Cdm.Figma
{
    public class FigmaDownloader : IFigmaDownloader
    {
        public async Task<JObject> DownloadFileAsync(string fileID, string personalAccessToken)
        {
            Debug.Log($"Downloading file: {fileID}");

            var fileContentJson = await FigmaApi.GetFileAsTextAsync(
                new FigmaFileRequest(personalAccessToken, fileID)
                {
                    geometry = "paths",
                    plugins = new[] { PluginData.Id }
                });


            var fileContent = JObject.Parse(fileContentJson);
            fileContent.Add(nameof(FigmaFile.fileID), fileID);

            if (fileContent.TryGetValue(nameof(FigmaFile.thumbnailUrl), out var thumbnailUrl))
            {
                var thumbnail = await FigmaApi.GetThumbnailImageAsync(thumbnailUrl.Value<string>());
                if (thumbnail != null)
                {
                    fileContent.Add(nameof(FigmaFile.thumbnail), Convert.ToBase64String(thumbnail));
                }
            }

            return fileContent;
        }
    }
}