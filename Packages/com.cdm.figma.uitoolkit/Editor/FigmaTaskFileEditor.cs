using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
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

        protected override Task OnFigmaFileSaved(Figma.FigmaTaskFile taskFile, Figma.FigmaFile file)
        {
            return Task.CompletedTask;
            //await SaveVectorGraphicsAsync((FigmaTaskFile) taskFile, (FigmaFile) file);
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
                foreach (var document in documents)
                {
                    // Save external style sheet file.
                    if (!string.IsNullOrEmpty(document.styleSheet))
                    {
                        var styleSheetPath = Path.Combine(assetsDirectory, $"{document.page.name}.uss");
                        File.WriteAllText(styleSheetPath, document.styleSheet);
                        
                        var styleElement = 
                            new XElement("Style", 
                                new XAttribute("src", $"project:///{styleSheetPath}".Replace("\\", "/")));
                        document.uxml.Root?.Add(styleElement);
                    }

                    // Generate scripts.
                    if (figmaTaskFile.generateScripts && document.script != null)
                    {
                        var scriptsDirectory = Path.Combine("Assets", figmaTaskFile.scriptsPath, document.page.name);
                        if (!Directory.Exists(scriptsDirectory))
                        {
                            Directory.CreateDirectory(scriptsDirectory);
                        }
                        
                        var scripPath = Path.Combine(scriptsDirectory, $"{document.script.name}.cs");
                        File.WriteAllText(scripPath, document.script.contents);
                    }

                    var documentDirectory = Path.Combine(assetsDirectory, document.page.name);
                    if (!Directory.Exists(documentDirectory))
                    {
                        Directory.CreateDirectory(documentDirectory);
                    }
                    
                    var documentPath = Path.Combine(documentDirectory, $"{document.node.name}.uxml");
                    using var fileStream = File.Create(documentPath);
                    using var xmlWriter = new XmlTextWriter(fileStream, Encoding.UTF8);
                    xmlWriter.Formatting = Formatting.Indented;
                    document.uxml.Save(xmlWriter);

                    Debug.Log($"UI document has been saved to: {documentPath}");    
                }
            });
            
            AssetDatabase.Refresh();
        }

       /* private static async Task SaveVectorGraphicsAsync(FigmaTaskFile taskFile, FigmaFile file)
        {
            if (!file.graphics.Any())
                return;
            
            var directory = Path.Combine("Assets", taskFile.graphicsPath);
            Directory.CreateDirectory(directory);

            foreach (var graphic in file.graphics)
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
        }*/
    }
}