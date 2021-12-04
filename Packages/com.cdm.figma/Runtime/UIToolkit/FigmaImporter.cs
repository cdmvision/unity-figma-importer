using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cdm.Figma.UIToolkit.UIToolkit;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class FigmaImporter : IFigmaImporter
    {
        private readonly HashSet<NodeConverter> _nodeConverters = new HashSet<NodeConverter>();

        public ISet<NodeConverter> nodeConverters => _nodeConverters;

        private readonly HashSet<ComponentConverter> _componentConverters = new HashSet<ComponentConverter>();

        public ISet<ComponentConverter> componentConverters => _componentConverters;

        
        private readonly List<KeyValuePair<FigmaFilePage, XDocument>> _documents 
            = new List<KeyValuePair<FigmaFilePage, XDocument>>();

        public KeyValuePair<FigmaFilePage, XDocument>[] GetImportedDocuments()
        {
            return _documents.ToArray();
        }
        
        public static NodeConverter[] GetDefaultNodeConverters()
        {
            return new NodeConverter[]
            {
                new GroupNodeConverter(),
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
                new ButtonComponentConverter(),
                new ToggleComponentConverter()
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

            foreach (var graphicId in graphicIds)
            {
                figmaFile.graphics.Add(new GraphicSource() { id = graphicId, graphic = null});
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

            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace ui = "UnityEngine.UIElements";
            XNamespace uie = "UnityEditor.UIElements";

            var fileContent = figmaFile.GetFileContent();

            var conversionArgs = new NodeConvertArgs(this, figmaFile, fileContent);
            conversionArgs.namespaces = new XNamespaces(ui, uie);

            // Build node hierarchy.
            fileContent.BuildHierarchy();

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

                var xml = new XDocument();
                
                var rootElement = new XElement(ui + "UXML");
                rootElement.Add(new XAttribute(XNamespace.Xmlns + nameof(xsi), xsi.NamespaceName));
                rootElement.Add(new XAttribute(XNamespace.Xmlns + nameof(ui), ui.NamespaceName));
                rootElement.Add(new XAttribute(xsi + "noNamespaceSchemaLocation",
                    "../../../../../UIElementsSchema/UIElements.xsd"));
                xml.Add(rootElement);

                // Add page element with ignoring the background color.
                var pageElement = NodeElement.New<VisualElement>(page, conversionArgs);
                pageElement.style.flexGrow = new StyleFloat(1f);
                pageElement.UpdateStyle();
                
                var nodes = page.children;
                foreach (var node in nodes)
                {
                    if (TryConvertNode(node, conversionArgs, out var data))
                    {
                        pageElement.value.Add(data.value);
                    }
                }
                
                rootElement.Add(pageElement.value);
                
                _documents.Add(new KeyValuePair<FigmaFilePage, XDocument>(filePage, xml));
            }

            return Task.CompletedTask;
        }

        internal bool TryConvertNode(Node node, NodeConvertArgs args, out NodeElement element)
        {
            // Try with component converters first.
            var componentConverter = componentConverters.FirstOrDefault(c => c.CanConvert(node, args));
            if (componentConverter != null)
            {
                element = componentConverter.Convert(node, args);
                return true;
            }

            // Try with node converters.
            var nodeConverter = nodeConverters.FirstOrDefault(c => c.CanConvert(node, args));
            if (nodeConverter != null)
            {
                element = nodeConverter.Convert(node, args);
                return true;
            }

            element = null;
            return false;
        }
    }
}