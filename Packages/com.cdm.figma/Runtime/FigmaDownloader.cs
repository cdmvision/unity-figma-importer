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
                    plugins = new []{ PluginData.Id }
                });
            
            var fileContent = FigmaFileContent.FromString(fileContentJson);
            var thumbnail = await FigmaApi.GetThumbnailImageAsync(fileContent.thumbnailUrl);
            return FigmaFile.Create<FigmaFile>(fileID, fileContentJson, thumbnail);
        }
    }
}