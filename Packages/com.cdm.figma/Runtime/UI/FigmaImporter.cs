using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class FigmaImporter : IFigmaImporter
    {
        private readonly HashSet<NodeConverter> _nodeConverters = new HashSet<NodeConverter>();

        public ISet<NodeConverter> nodeConverters => _nodeConverters;

        private readonly HashSet<ComponentConverter> _componentConverters = new HashSet<ComponentConverter>();

        public ISet<ComponentConverter> componentConverters => _componentConverters;
        
        private readonly List<ImportedDocument> _documents = new List<ImportedDocument>();

        public ImportedDocument[] GetImportedDocuments()
        {
            return _documents.ToArray();
        }
        
        public static NodeConverter[] GetDefaultNodeConverters()
        {
            return new NodeConverter[]
            {
                new GroupNodeConverter(),
                new FrameNodeConverter(),
                new InstanceNodeConverter(),
                new VectorNodeConverter(),
                new RectangleNodeConverter(),
                new EllipseNodeConverter(),
                new LineNodeConverter(),
                new PolygonNodeConverter(),
                new StarNodeConverter(),
                new TextNodeConverter()
            };
        }

        public static ComponentConverter[] GetDefaultComponentConverters()
        {
            return new ComponentConverter[]
            {
                //new ButtonComponentConverter(),
                //new ToggleComponentConverter()
            };
        }
        
        public Figma.FigmaFile CreateFile(string fileId, string fileJson, byte[] thumbnailData = null)
        {
            var fileContent = FigmaFileContent.FromString(fileJson);

            var figmaFile = FigmaFile.CreateInstance<FigmaFile>();
            figmaFile.id = fileId;
            figmaFile.title = fileContent.name;
            figmaFile.version = fileContent.version;
            figmaFile.lastModified = fileContent.lastModified.ToString("u");
            figmaFile.content = new TextAsset(JObject.Parse(fileJson).ToString(Newtonsoft.Json.Formatting.Indented));
            figmaFile.content.name = "File";

            if (thumbnailData != null)
            {
                figmaFile.thumbnail = new Texture2D(1, 1);
                figmaFile.thumbnail.name = "Thumbnail";
                figmaFile.thumbnail.LoadImage(thumbnailData);
            }

            var pages = fileContent.document.children;
            figmaFile.pages = new FigmaFilePage[pages.Length];

            for (var i = 0; i < pages.Length; i++)
            {
                figmaFile.pages[i] = new FigmaFilePage()
                {
                    id = pages[i].id,
                    name = pages[i].name
                };
            }

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

        public Task ImportFileAsync(Figma.FigmaFile file)
        {
            _documents.Clear();
            
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            
            var figmaFile = file as FigmaFile;
            if (figmaFile == null)
                throw new ArgumentException("Wrong type of Figma file", nameof(file));
            
            if (!nodeConverters.Any())
            {
                var converters = GetDefaultNodeConverters();
                foreach (var converter in converters)
                {
                    nodeConverters.Add(converter);
                }
            }
            
            if (!componentConverters.Any())
            {
                var converters = GetDefaultComponentConverters();
                foreach (var converter in converters)
                {
                    componentConverters.Add(converter);
                }
            }
            
            var fileContent = figmaFile.GetFileContent();
            fileContent.BuildHierarchy();
            
            var conversionArgs = new NodeConvertArgs(this, figmaFile, fileContent);
            
            // Collect all component sets from all pages.
            var pages = fileContent.document.children;
            foreach (var page in pages)
            {
                page.Traverse(node =>
                {
                    conversionArgs.componentSets.Add((ComponentSetNode) node);
                    return true;
                }, NodeType.ComponentSet);
            }
            
            // Generate all pages.
            foreach (var page in pages)
            {
                // Do not import ignored pages.
                var filePage = figmaFile.pages.FirstOrDefault(p => p.id == page.id);
                if (filePage == null || !filePage.enabled)
                    continue;

                var pageNode = NodeObject.NewNodeObject(page, conversionArgs);
                pageNode.rectTransform.anchorMin = new Vector2(0, 0);
                pageNode.rectTransform.anchorMax = new Vector2(1, 1);
                pageNode.rectTransform.offsetMin = new Vector2(0, 0);
                pageNode.rectTransform.offsetMax = new Vector2(1, 1);

                var nodes = page.children;
                foreach (var node in nodes)
                {
                    if (node is FrameNode)
                    {
                        if (TryConvertNode(pageNode, node, conversionArgs, out var frameNode))
                        {
                            frameNode.rectTransform.anchorMin = new Vector2(0, 0);
                            frameNode.rectTransform.anchorMax = new Vector2(1, 1);
                            frameNode.rectTransform.offsetMin = new Vector2(0, 0);
                            frameNode.rectTransform.offsetMax = new Vector2(1, 1);
                            frameNode.transform.SetParent(pageNode.rectTransform, false);
                        }
                    }
                }
                
                _documents.Add(new ImportedDocument()
                {
                    page = filePage,
                    node = page,
                    nodeObject = pageNode
                });
            }

            return Task.CompletedTask;
        }
        
        internal bool TryConvertNode(NodeObject parentObject, Node node, NodeConvertArgs args, out NodeObject nodeObject)
        {
            // Try with component converters first.
            var componentConverter = componentConverters.FirstOrDefault(c => c.CanConvert(node, args));
            if (componentConverter != null)
            {
                nodeObject = componentConverter.Convert(parentObject, node, args);
                return true;
            }

            // Try with node converters.
            var nodeConverter = nodeConverters.FirstOrDefault(c => c.CanConvert(node, args));
            if (nodeConverter != null)
            {
                nodeObject = nodeConverter.Convert(parentObject, node, args);
                return true;
            }

            nodeObject = null;
            return false;
        }
        
        public struct ImportedDocument
        {
            /// <summary>
            /// Associated Figma page.
            /// </summary>
            public FigmaFilePage page;

            /// <summary>
            /// Root figma node.
            /// </summary>
            public Node node;
        
            /// <summary>
            /// Root game object..
            /// </summary>
            public NodeObject nodeObject;
        }
        
        /*public override Task ImportFileAsync(FigmaFile file, FigmaImportOptions options = null)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            options ??= new FigmaImportOptions();
            
            var assetsDirectory = Path.Combine("Assets", assetsPath);
            Directory.CreateDirectory(assetsDirectory);

            var conversionArgs = new NodeConvertArgs(this, file);
            conversionArgs.assets = options.assets;
            
            // Collect all component sets from all pages.
            var pages = file.document.children;
            foreach (var page in pages)
            {
                page.Traverse(node =>
                {
                    conversionArgs.componentSets.Add((ComponentSetNode) node);
                    return true;
                }, NodeType.ComponentSet);
            }
            
            // Generate all pages.
            foreach (var page in pages)
            {
                if (options.pages != null && options.pages.All(p => p != page.id))
                    continue;

                var nodeObject = NodeObject.NewNodeObject(page, conversionArgs);
                var canvas = nodeObject.gameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                
                var nodes = page.children;
                foreach (var node in nodes)
                {
                    if (TryConvertNode(node, conversionArgs, out var childNode))
                    {
                        childNode.transform.SetParent(canvas.transform);
                    }
                }
            }
            
            
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            return Task.CompletedTask;
        }*/
    }
}