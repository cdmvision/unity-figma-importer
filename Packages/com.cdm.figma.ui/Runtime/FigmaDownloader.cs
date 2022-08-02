using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class FigmaDownloader : IFigmaDownloader
    {
        public async Task<Figma.FigmaFile> DownloadFileAsync(string fileID, string personalAccessToken)
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
            var figmaFile = FigmaFile.Create<FigmaFile>(fileID, fileContentJson, thumbnail);
            
            AddUsedFonts(figmaFile, fileContent);
            
            return figmaFile;
        }
        
        private static void AddUsedFonts(FigmaFile file, FigmaFileContent fileContent)
        {
            var fontSet = new HashSet<string>();
            fileContent.document.Traverse(node =>
            {
                var style = ((TextNode) node).style;
                var fontName = FontSource.GetFontName(style.fontFamily, style.fontWeight, style.italic);
                fontSet.Add(fontName);
                return true;
            }, NodeType.Text);

            var fonts = fontSet.ToArray();

            file.fonts = new FontSource[fonts.Length];
            for (var i = 0; i < file.fonts.Length; i++)
            {
                file.fonts[i] = new FontSource() { fontName = fonts[i], font = null };
            }
        }
    }
}