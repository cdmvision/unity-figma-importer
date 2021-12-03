using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Unity.VectorGraphics.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    [CustomEditor(typeof(FigmaTaskFile))]
    public class FigmaTaskFileEditor : Cdm.Figma.FigmaTaskFileEditor
    {
        protected override string GetImplementationName() => nameof(UIToolkit);

        protected override Task OnFigmaFileSaving(
            Figma.FigmaTaskFile taskFile, Figma.FigmaFile newFile, Figma.FigmaFile oldFile)
        {
            if (oldFile != null)
            {
                var oldFigmaFile = (FigmaFile) oldFile;
                var newFigmaFile = (FigmaFile) newFile;

                newFigmaFile.fallbackFont = oldFigmaFile.fallbackFont;

                foreach (var fontSource in newFigmaFile.fonts)
                {
                    if (fontSource.font == null)
                    {
                        var oldFont = oldFigmaFile.fonts.FirstOrDefault(x =>
                            x.fontName.Equals(fontSource.fontName, StringComparison.OrdinalIgnoreCase));
                        if (oldFont != null && oldFont.font != null)
                        {
                            fontSource.font = oldFont.font;
                        }
                    }
                }
            }
            
            return Task.CompletedTask;
        }

        protected override async Task OnFigmaFileSaved(
            Figma.FigmaTaskFile taskFile, Figma.FigmaFile file, FigmaFileContent fileContent)
        {
            await SaveVectorGraphicsAsync((FigmaTaskFile) taskFile, (FigmaFile) file, fileContent);
        }

        protected override async Task OnFigmaFileImported(Figma.FigmaTaskFile taskFile, Figma.FigmaFile file)
        {
            await Task.Run(() =>
            {
                var figmaTaskFile = (FigmaTaskFile) taskFile;
                
                var documentsPath = figmaTaskFile.documentsPath;
                var assetsDirectory = Path.Combine("Assets", documentsPath);
                Directory.CreateDirectory(assetsDirectory);
                
                var documents = figmaTaskFile.importer.GetImportedDocuments();
                foreach (var (page, document) in documents)
                {
                    var documentPath = Path.Combine(assetsDirectory, $"{page.name}.uxml");
                    using var fileStream = File.Create(documentPath);
                    using var xmlWriter = new XmlTextWriter(fileStream, Encoding.UTF8);
                    xmlWriter.Formatting = Formatting.Indented;
                    document.Save(xmlWriter);

                    Debug.Log($"UI document has been saved to: {documentPath}");    
                }
            });
            
            AssetDatabase.Refresh();
        }

        private static async Task SaveVectorGraphicsAsync(
            FigmaTaskFile taskFile, FigmaFile file, FigmaFileContent fileContent)
        {
            if (!file.graphics.Any())
                return;
            
            var graphics = 
                await FigmaApi.GetImageAsync(new FigmaImageRequest(taskFile.personalAccessToken, file.id)
                {
                    ids = file.graphics.Select(x => x.id).ToArray(),
                    format = "svg",
                    svgIncludeId = false,
                    svgSimplifyStroke = true
                });
            
            var directory = Path.Combine("Assets", taskFile.graphicsPath);
            Directory.CreateDirectory(directory);

            foreach (var graphic in graphics)
            {
                if (graphic.Value != null)
                {
                    var fileName = $"{graphic.Key.Replace(":", "-").Replace(";", "_")}.svg";
                    
                    var path = Path.Combine(Application.dataPath, taskFile.graphicsPath, fileName);
                    await File.WriteAllBytesAsync(path, graphic.Value);

                    var graphicPath = Path.Combine("Assets", taskFile.graphicsPath, fileName);
                    AssetDatabase.ImportAsset(graphicPath, ImportAssetOptions.ForceSynchronousImport);

                    var svgImporter = (SVGImporter) AssetImporter.GetAtPath(graphicPath);
                    svgImporter.PreserveSVGImageAspect = true;
                    svgImporter.SvgType = SVGType.UIToolkit;

                    EditorUtility.SetDirty(svgImporter);
                    svgImporter.SaveAndReimport();
                    
                    var vectorImage = AssetDatabase.LoadAssetAtPath<VectorImage>(graphicPath);
                    var graphicSource = file.graphics.First(x => x.id == graphic.Key);
                    graphicSource.graphic = vectorImage;
                }
                else
                {
                    Debug.LogWarning($"Graphic could not be rendered: {graphic.Key}");
                }
            }
            
            EditorUtility.SetDirty(file);
        }
    }
}