using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
        
        private readonly List<ImportedDocument> _documents = new List<ImportedDocument>();

        public IScriptGenerator scriptGenerator { get; set; } = new ScriptGenerator();
        
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

                var nodes = page.children;
                foreach (var node in nodes)
                {
                    if (node is FrameNode)
                    {
                        if (TryConvertNode(node, conversionArgs, out var root))
                        {
                            root.inlineStyle.width = new StyleLength(new Length(100f, LengthUnit.Percent));
                            root.inlineStyle.height = new StyleLength(new Length(100f, LengthUnit.Percent));

                            var importedDocument = new ImportedDocument();
                            importedDocument.page = filePage;
                            importedDocument.node = node;
                            
                            var (uxml, styleSheet) = BuildUxml(root, conversionArgs.namespaces);
                            importedDocument.uxml = uxml;
                            importedDocument.styleSheet = styleSheet;
                            
                            if (scriptGenerator != null)
                            {
                                importedDocument.script = scriptGenerator.Generate(root);
                            }
                        
                            _documents.Add(importedDocument);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
        
        private static (XDocument, string) BuildUxml(NodeElement rootNode, XNamespaces namespaces)
        {
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            
            var xml = new XDocument();
            var rootElement = new XElement(namespaces.engine + "UXML");
            rootElement.Add(new XAttribute(XNamespace.Xmlns + nameof(xsi), xsi.NamespaceName));
            rootElement.Add(new XAttribute(XNamespace.Xmlns + nameof(namespaces.engine), namespaces.engine.NamespaceName));
            rootElement.Add(new XAttribute(xsi + "noNamespaceSchemaLocation",
                "../../../../../UIElementsSchema/UIElements.xsd"));
            xml.Add(rootElement);

            var style = new StringBuilder();
            
            BuildUxmlHierarchy(rootNode, style);
            
            rootElement.Add(rootNode.value);
            return (xml, style.ToString());
        }
        
        private static void BuildUxmlHierarchy(NodeElement element, StringBuilder style)
        {
            if (element.styles.Any())
            {
                // Save styles in the stylesheet file.
                foreach (var styleDefinition in element.styles)
                {
                    style.AppendLine($"{styleDefinition.selector} {{ {styleDefinition.style} }}");
                }
            }
            else
            {
                element.value.SetAttributeValue("style", element.inlineStyle.ToString());    
            }

            foreach (var child in element.children)
            {
                element.value.Add(child.value);
                
                BuildUxmlHierarchy(child, style);
            }
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
        /// Generated UXML layout file.
        /// </summary>
        public XDocument uxml;
        
        /// <summary>
        /// Generated USS style sheet file.
        /// </summary>
        public string styleSheet;
        
        /// <summary>
        /// Generated script file.
        /// </summary>
        public GeneratedScript script;
    }
}