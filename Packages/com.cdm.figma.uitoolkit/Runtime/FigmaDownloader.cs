using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    public class FigmaDownloader : IFigmaDownloader
    {
        public async Task<Figma.FigmaFile> DownloadFileAsync(string fileID, string personalAccessToken)
        {
            var fileContentJson = await FigmaApi.GetFileAsTextAsync(
                new FigmaFileRequest(personalAccessToken, fileID)
                {
                    geometry = "paths",
                    plugins = new []{ PluginData.Id }
                });
            
            var fileContent = FigmaFileContent.FromString(fileContentJson);
            var thumbnail = await FigmaApi.GetThumbnailImageAsync(fileContent.thumbnailUrl);

            var figmaFile = FigmaFile.Create<FigmaFile>(fileID, fileContentJson, thumbnail);

            await DownloadGraphicsAsync(figmaFile, fileContent, personalAccessToken);

            // Add required fonts.
            var fonts = new HashSet<string>();
            fileContent.document.Traverse(node =>
            {
                var style = ((TextNode) node).style;
                var fontName = FontSource.GetFontName(style.fontFamily, style.fontWeight, style.italic);
                fonts.Add(fontName);
                return true;
            }, NodeType.Text);

            foreach (var font in fonts)
            {
                figmaFile.fonts.Add(new FontSource() {fontName = font, font = null});
            }

            return figmaFile;
        }
        
        private static async Task DownloadGraphicsAsync(
            FigmaFile file, FigmaFileContent fileContent, string personalAccessToken)
        {
            // Add required graphics.
            var graphicIds = new HashSet<string>();
            fileContent.document.Traverse(node =>
            {
                // Invisible nodes cannot be rendered.
                if (node is SceneNode sceneNode && sceneNode.visible)
                {
                    graphicIds.Add(sceneNode.id);
                }
                return true;
            }, NodeType.Vector, NodeType.Ellipse, NodeType.Line, NodeType.Polygon, NodeType.Star);

            var graphics = await FigmaApi.GetImageAsync(new FigmaImageRequest(personalAccessToken, file.id)
            {
                ids = graphicIds.ToArray(),
                format = "svg",
                svgIncludeId = true,
                svgSimplifyStroke = true,
                version = file.version
            });

            foreach (var graphic in graphics)
            {
                if (graphic.Value != null)
                {
                    file.graphics.Add(new GraphicSource()
                    {
                        id = graphic.Key,
                        content = Encoding.UTF8.GetString(graphic.Value)
                    });
                }
                else
                {
                    Debug.LogWarning($"Graphic could not be rendered: {graphic.Key}");
                }
            }
        }
    }
}