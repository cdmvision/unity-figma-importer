using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
        
        public List<FontSource> fonts { get; } = new List<FontSource>();
        
        /// <summary>
        /// Gets or sets the fallback font that is used when a font mapping does not found.
        /// </summary>
        public TMP_FontAsset fallbackFont { get; set; }

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
                new VectorNodeConverter(),
                new RectangleNodeConverter(),
                new EllipseNodeConverter(),
                new LineNodeConverter(),
                new PolygonNodeConverter(),
                new StarNodeConverter(),
                new TextNodeConverter(),
                new InstanceNodeConverter()
            };
        }

        public static ComponentConverter[] GetDefaultComponentConverters()
        {
            return new ComponentConverter[]
            {
                new UnknownComponentConverter(),
                new ButtonComponentConverter(),
                new ToggleComponentConverter(),
                new SliderComponentConverter(),
                new InputFieldComponentConverter(),
                new ScrollbarComponentConverter(),
                new ScrollViewComponentConverter(),
                new DropdownComponentConverter()
            };
        }
        
        public void ImportFile(FigmaFile file)
        {
            _documents.Clear();
            
            if (file == null)
                throw new ArgumentNullException(nameof(file));

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
            
            var fileContent = file.GetFileContent();
            fileContent.BuildHierarchy();
            
            var conversionArgs = new NodeConvertArgs(this, file, fileContent);
            
            // Generate all pages.
            var pages = fileContent.document.children;
            
            foreach (var page in pages)
            {
                // Do not import ignored pages.
                var filePage = file.pages.FirstOrDefault(p => p.id == page.id);
                if (filePage == null || !filePage.enabled)
                    continue;

                var pageNode = NodeObject.Create<PageNodeObject>(page, conversionArgs);
                pageNode.rectTransform.anchorMin = new Vector2(0, 0);
                pageNode.rectTransform.anchorMax = new Vector2(1, 1);
                pageNode.rectTransform.offsetMin = new Vector2(0, 0);
                pageNode.rectTransform.offsetMax = new Vector2(0, 0);
                
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
                            frameNode.rectTransform.offsetMax = new Vector2(0, 0);
                            frameNode.transform.SetParent(pageNode.rectTransform, false);
                        }
                    }
                }
                
                _documents.Add(new ImportedDocument()
                {
                    page = filePage,
                    node = page,
                    nodeObject = pageNode,
                    sprites = conversionArgs.generatedSprites.Values.ToArray(),
                    materials = conversionArgs.generatedMaterials.ToArray()
                });

                conversionArgs.generatedSprites.Clear();
                conversionArgs.generatedMaterials.Clear();
            }
        }

        internal bool TryConvertNode(NodeObject parentObject, Node node, NodeConvertArgs args, 
            out NodeObject nodeObject)
        {
            // Init instance node's main component, and main component's component set.
            if (node is InstanceNode instanceNode)
            {
                args.fileContent.InitInstanceNode(instanceNode);
            }
            
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
        
        internal bool TryGetFont(string fontName, out TMP_FontAsset font)
        {
            var fontIndex = fonts.FindIndex(
                x => string.Equals(x.fontName, fontName, StringComparison.OrdinalIgnoreCase));

            if (fontIndex >= 0 && fonts[fontIndex].font != null)
            {
                font = fonts[fontIndex].font;
                return true;
            }

            if (fallbackFont != null)
            {
                font = fallbackFont;
                return true;
            }

            font = null;
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
            /// Root game object.
            /// </summary>
            public PageNodeObject nodeObject;
            
            /// <summary>
            /// Generated sprites.
            /// </summary>
            public Sprite[] sprites;
            
            /// <summary>
            /// Generated materials.
            /// </summary>
            public Material[] materials;
        }
    }
}