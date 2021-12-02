using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(UIToolkit) + "-" + nameof(FigmaImporter),
        menuName = AssetMenuRoot + "Figma Importer", order = 0)]
    public class FigmaImporter : Figma.FigmaImporter
    {
        public new const string AssetMenuRoot = Figma.FigmaImporter.AssetMenuRoot + "UI Toolkit/";

        [SerializeField]
        private string _assetsPath = "Resources/Figma/Documents";

        public string assetsPath => _assetsPath;

        [SerializeField]
        private List<NodeConverter> _nodeConverters = new List<NodeConverter>();

        public List<NodeConverter> nodeConverters => _nodeConverters;

        [SerializeField]
        private List<ComponentConverter> _componentConverters = new List<ComponentConverter>();

        public List<ComponentConverter> componentConverters => _componentConverters;

        public override Figma.FigmaFile CreateFile(string fileId, string fileJson, byte[] thumbnailData = null)
        {
            var fileContent = FigmaFileContent.FromString(fileJson);

            var figmaFile = CreateInstance<FigmaFile>();
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

        public override async Task ImportFileAsync(Figma.FigmaFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var figmaFile = file as FigmaFile;
            if (figmaFile == null)
                throw new ArgumentException("Wrong type of Figma file", nameof(file));

            var assetsDirectory = Path.Combine("Assets", assetsPath);
            Directory.CreateDirectory(assetsDirectory);

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
                if (figmaFile.pages.Any(p => p.id == page.id && !p.enabled))
                    continue;

                var xml = new XDocument();
                var root = new XElement(ui + "UXML");
                root.Add(new XAttribute(XNamespace.Xmlns + nameof(xsi), xsi.NamespaceName));
                root.Add(new XAttribute(XNamespace.Xmlns + nameof(ui), ui.NamespaceName));
                root.Add(new XAttribute(xsi + "noNamespaceSchemaLocation",
                    "../../../../../UIElementsSchema/UIElements.xsd"));
                xml.Add(root);

                // Add page element with ignoring the background color.
                var pageElement = XmlFactory.NewElement<VisualElement>(page, conversionArgs).Style($"flex-grow: 1;");
                root.Add(pageElement);

                var nodes = page.children;
                foreach (var node in nodes)
                {
                    if (TryConvertNode(node, conversionArgs, out var element))
                    {
                        pageElement.Add(element);
                    }
                }

                await Task.Run(() =>
                {
                    var documentPath = Path.Combine(assetsDirectory, $"{page.name}.uxml");
                    using var fileStream = File.Create(documentPath);
                    using var xmlWriter = new XmlTextWriter(fileStream, Encoding.UTF8);
                    xmlWriter.Formatting = Formatting.Indented;
                    xml.Save(xmlWriter);

                    Debug.Log($"UI document has been saved to: {documentPath}");
                });
            }

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        internal bool TryConvertNode(Node node, NodeConvertArgs args, out XElement element)
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