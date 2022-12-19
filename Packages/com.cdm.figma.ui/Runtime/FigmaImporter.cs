using System;
using System.Collections.Generic;
using System.Linq;
using Cdm.Figma.UI.Utils;
using Cdm.Figma.Utils;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cdm.Figma.UI
{
    /// <summary>
    /// Imports given Figma file into the Unity using built-in UI system.
    /// </summary>
    public class FigmaImporter : IFigmaImporter
    {
        private readonly List<INodeConverter> _nodeConverters = new List<INodeConverter>();

        /// <summary>
        /// Figma node converters.
        /// </summary>
        /// <remarks>If left empty, default node converters are used.</remarks>
        /// <seealso cref="GetDefaultNodeConverters"/>
        /// <seealso cref="AddDefaultNodeConverters"/>
        public IList<INodeConverter> nodeConverters => _nodeConverters;

        private readonly List<ComponentConverter> _componentConverters = new List<ComponentConverter>();

        /// <summary>
        /// Figma component converters.
        /// </summary>
        /// <remarks>If left empty, default component converters are used.</remarks>
        /// <seealso cref="GetDefaultComponentConverters"/>
        /// <seealso cref="AddDefaultComponentConverters"/>
        public IList<ComponentConverter> componentConverters => _componentConverters;
        
        private readonly List<IEffectConverter> _effectConverters = new List<IEffectConverter>();
        
        /// <summary>
        /// Figma effect converters.
        /// </summary>
        /// <seealso cref="BlurEffect"/>
        /// <seealso cref="ShadowEffect"/>
        public IList<IEffectConverter> effectConverters => _effectConverters;
        
        private readonly List<FigmaImporterLogReference> _logs = new List<FigmaImporterLogReference>();
        private readonly List<Binding> _bindings = new List<Binding>();

        /// <summary>
        /// The font mappings that are used while importing Figma text nodes.
        /// </summary>
        /// <seealso cref="TextNode"/>
        public List<FontSource> fonts { get; } = new List<FontSource>();
        
        /// <summary>
        /// Generated UI elements as <see cref="GameObject"/>.
        /// </summary>
        public AssetCache generatedGameObjects { get; } = new AssetCache();

        /// <summary>
        /// Generated assets during import such as <see cref="Sprite"/>, <see cref="Material"/> etc.
        /// </summary>
        public AssetCache generatedAssets { get; } = new AssetCache();

        /// <summary>
        /// Dependency assets that are used to import Figma nodes such as font asset etc.
        /// </summary>
        /// <seealso cref="FontSource"/>
        public AssetCache dependencyAssets { get; } = new AssetCache();

        /// <summary>
        /// Gets or sets whether import process fails or keeps a log when an error occurs.
        /// </summary>
        public bool failOnError { get; set; } = true;

        /// <summary>
        /// Sprite generation options.
        /// </summary>
        public SpriteGenerateOptions spriteOptions { get; set; } = new SpriteGenerateOptions()
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
            sampleCount = 8,
            textureSize = 1024
        };

        /// <summary>
        /// Gets or sets the fallback font that is used when a font mapping does not found.
        /// </summary>
        public TMP_FontAsset fallbackFont { get; set; }

        /// <summary>
        /// Gets the default node converters that are used for importing Figma nodes.
        /// </summary>
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
                new InstanceNodeConverter(),
                new BooleanNodeConverter()
            };
        }

        /// <summary>
        /// Gets the default component converters that are used for importing Figma nodes.
        /// </summary>
        public static ComponentConverter[] GetDefaultComponentConverters()
        {
            return new ComponentConverter[]
            {
                new SelectableComponentConverter(),
                new ButtonComponentConverter(),
                new ToggleComponentConverter(),
                new SliderComponentConverter(),
                new InputFieldComponentConverter(),
                new ScrollbarComponentConverter(),
                new ScrollViewComponentConverter(),
                new DropdownComponentConverter()
            };
        }

        /// <summary>
        /// Adds the default node converters that are used for importing Figma nodes.
        /// </summary>
        /// <seealso cref="GetDefaultNodeConverters"/>
        public void AddDefaultNodeConverters()
        {
            var converters = GetDefaultNodeConverters();
            foreach (var converter in converters)
            {
                nodeConverters.Add(converter);
            }
        }

        /// <summary>
        /// Adds the default component converters that are used for importing Figma nodes.
        /// </summary>
        /// <seealso cref="GetDefaultComponentConverters"/>
        public void AddDefaultComponentConverters()
        {
            var converters = GetDefaultComponentConverters();
            foreach (var converter in converters)
            {
                componentConverters.Add(converter);
            }
        }

        public Figma.FigmaDesign ImportFile(FigmaFile file, IFigmaImporter.Options options = null)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            options ??= new IFigmaImporter.Options();
            spriteOptions ??= new SpriteGenerateOptions();

            file.BuildHierarchy();

            generatedAssets.Clear();
            generatedGameObjects.Clear();
            dependencyAssets.Clear();

            _logs.Clear();
            _bindings.Clear();

            InitNodeConverters();
            InitComponentConverters();

            try
            {
                var conversionArgs = new NodeConvertArgs(this, file);

                var figmaDesign = FigmaDesign.Create<FigmaDesign>(file);
                var figmaDocument = CreateFigmaNode<FigmaDocument>(file.document, figmaDesign.gameObject);
                figmaDocument.rectTransform.anchorMin = new Vector2(0, 0);
                figmaDocument.rectTransform.anchorMax = new Vector2(1, 1);
                figmaDocument.rectTransform.offsetMin = new Vector2(0, 0);
                figmaDocument.rectTransform.offsetMax = new Vector2(0, 0);

                var pageNodes = file.document.children;
                foreach (var pageNode in pageNodes)
                {
                    // Do not import ignored pages.
                    if (options.selectedPages != null && options.selectedPages.All(p => p != pageNode.id))
                        continue;

                    var figmaPage = CreateFigmaNode<FigmaPage>(pageNode);
                    figmaPage.rectTransform.anchorMin = new Vector2(0, 0);
                    figmaPage.rectTransform.anchorMax = new Vector2(1, 1);
                    figmaPage.rectTransform.offsetMin = new Vector2(0, 0);
                    figmaPage.rectTransform.offsetMax = new Vector2(0, 0);
                    figmaPage.transform.SetParent(figmaDocument.rectTransform, false);

                    AddBindingIfExist(figmaPage);

                    var nodes = pageNode.children;
                    foreach (var node in nodes)
                    {
                        if (node.IsIgnored())
                            continue;

                        if (node is FrameNode)
                        {
                            if (TryConvertNode(figmaPage, node, conversionArgs, out var frameNode))
                            {
                                frameNode.rectTransform.anchorMin = new Vector2(0, 0);
                                frameNode.rectTransform.anchorMax = new Vector2(1, 1);
                                frameNode.rectTransform.offsetMin = new Vector2(0, 0);
                                frameNode.rectTransform.offsetMax = new Vector2(0, 0);
                                frameNode.transform.SetParent(figmaPage.rectTransform, false);
                            }
                        }
                    }
                }

                figmaDocument.InitPages();
                SetDocumentLogs(figmaDocument, _logs);
                _logs.Clear();

                var bindings = new List<Binding>();
                foreach (var binding in _bindings)
                {
                    if (binding.node != null)
                    {
                        // Build path.
                        var node = binding.node.node;
                        var path = BuildBindingPath(node);

                        bindings.Add(new Binding(binding.key, path, binding.node));
                    }
                }

                figmaDocument.TraverseDfs(node =>
                {
                    node.figmaDesign = figmaDesign;
                    return true;
                });

                figmaDesign.SetBindings(bindings);
                return figmaDesign;
            }
            catch (Exception)
            {
                // In case an error, cleanup generated resources.
                foreach (var generatedObject in generatedGameObjects)
                {
                    if (generatedObject.Value != null)
                    {
                        ObjectUtils.Destroy(generatedObject.Value);
                    }
                }

                foreach (var generatedAsset in generatedAssets)
                {
                    if (generatedAsset.Value != null)
                    {
                        ObjectUtils.Destroy(generatedAsset.Value);
                    }
                }

                generatedAssets.Clear();
                generatedGameObjects.Clear();
                dependencyAssets.Clear();
                throw;
            }
        }

        private static string BuildBindingPath(Node n)
        {
            var path = n.id;
            for (var node = n.parent; node != null; node = node.parent)
            {
                path = $"{node.id}{Binding.PathSeparator}{path}";
            }

            return path;
        }

        private void AddBindingIfExist(FigmaNode figmaNode)
        {
            if (!string.IsNullOrEmpty(figmaNode.bindingKey))
            {
                _bindings.Add(new Binding(figmaNode.bindingKey, "", figmaNode));
            }
        }

        internal bool TryConvertNode(FigmaNode parentObject, Node node, NodeConvertArgs args,
            out FigmaNode nodeObject, params INodeConverter[] ignoredConverters)
        {
            // Init instance node's main component, and main component's component set.
            var instanceNode = (InstanceNode)null;
            var instanceNodeInitResult = InstanceNodeInitResult.Success;
            if (node is InstanceNode ins)
            {
                instanceNode = ins;
                instanceNodeInitResult = args.file.InitInstanceNode(instanceNode);
            }

            // Try with component converters first.
            var componentConverter = componentConverters.FirstOrDefault(
                c => !ignoredConverters.Contains(c) && c.CanConvert(node, args));

            if (componentConverter != null)
            {
                nodeObject = componentConverter.Convert(parentObject, node, args);
                AddBindingIfExist(nodeObject);
                LogInstanceNodeInitResult(instanceNode, nodeObject, instanceNodeInitResult);
                return true;
            }

            // Try with node converters.
            var nodeConverter = nodeConverters.FirstOrDefault(
                c => !ignoredConverters.Contains(c) && c.CanConvert(node, args));

            if (nodeConverter != null)
            {
                nodeObject = nodeConverter.Convert(parentObject, node, args);
                AddBindingIfExist(nodeObject);
                LogInstanceNodeInitResult(instanceNode, nodeObject, instanceNodeInitResult);
                return true;
            }

            nodeObject = null;
            return false;
        }

        internal void ConvertEffects(FigmaNode node, IEnumerable<Effect> effects)
        {
            foreach (var effect in effects)
            {
                TryConvertEffect(node, effect);
            }
        }

        internal bool TryConvertEffect(FigmaNode node, Effect effect)
        {
            foreach (var effectConverter in effectConverters)
            {
                if (effectConverter.CanConvert(node, effect))
                {
                    effectConverter.Convert(node, effect);
                    return true;
                }
            }

            return false;
        }

        private void LogInstanceNodeInitResult(InstanceNode node, FigmaNode nodeObject, InstanceNodeInitResult result)
        {
            if (node == null)
                return;

            switch (result)
            {
                case InstanceNodeInitResult.MissingComponentID:
                    LogWarning($"Instance node has missing component ID. " +
                               $"Instance node {node} may not be imported properly.", nodeObject);
                    break;
                case InstanceNodeInitResult.MissingComponent:
                    LogWarning($"Instance of component node with id: '{node.componentId}' could not be found. " +
                               $"Instance node {node} may not be imported properly. " +
                               "Did you download file dependencies?", nodeObject);
                    break;
                case InstanceNodeInitResult.MissingComponentDefinition:
                    LogWarning($"Component definition could not be found for component node: '{node.componentId}'. " +
                               $"Instance node {node} may not be imported properly.", nodeObject);
                    break;
                case InstanceNodeInitResult.MissingComponentSet:
                    LogWarning($"Component set node of component node: '{node.componentId}' could not be found. " +
                               $"Instance node {node} may not be imported properly.", nodeObject);
                    break;
                case InstanceNodeInitResult.Success:
                default:
                    break;
            }
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

        internal T CreateFigmaNode<T>(Node node, GameObject existingGameObject = null) where T : FigmaNode
        {
            var figmaNode = FigmaNode.Create<T>(node, existingGameObject);
            generatedGameObjects.Add(figmaNode.nodeId, figmaNode.gameObject);
            return figmaNode;
        }

        internal void DestroyFigmaNode(FigmaNode figmaNode)
        {
            if (figmaNode != null)
            {
                generatedGameObjects.Remove<GameObject>(figmaNode.nodeId);
                ObjectUtils.Destroy(figmaNode.gameObject);
            }
        }

        internal void LogWarning(string message, Object target = null)
        {
            Debug.LogWarning(message, target);

            _logs.Add(new FigmaImporterLogReference(
                new FigmaImporterLog(FigmaImporterLogType.Warning, message), target));
        }

        internal void LogError(Exception exception, Object target = null)
        {
            if (failOnError)
            {
                throw exception;
            }

            Debug.LogError(exception, target);
            _logs.Add(new FigmaImporterLogReference(
                new FigmaImporterLog(FigmaImporterLogType.Error, exception.Message), target));
        }

        internal void LogError(string message, Object target = null)
        {
            LogError(new Exception(message), target);
        }

        private static void SetDocumentLogs(FigmaDocument figmaDocument,
            IEnumerable<FigmaImporterLogReference> logReferences)
        {
            foreach (var logReference in logReferences)
            {
                if (logReference.target != null)
                {
                    GameObject targetGameObject = null;

                    if (logReference.target is GameObject gameObject)
                    {
                        targetGameObject = gameObject;
                    }
                    else if (logReference.target is UnityEngine.Component component)
                    {
                        targetGameObject = component.gameObject;
                    }

                    if (targetGameObject != null)
                    {
                        var figmaNode = targetGameObject.GetComponent<FigmaNode>();
                        if (figmaNode != null)
                        {
                            figmaNode.logs.Add(logReference.log);
                        }
                    }
                }

                figmaDocument.allLogs.Add(logReference);
            }
        }

        private void InitNodeConverters()
        {
            if (!nodeConverters.Any())
            {
                AddDefaultNodeConverters();
            }
        }

        private void InitComponentConverters()
        {
            if (!componentConverters.Any())
            {
                AddDefaultComponentConverters();
            }
        }
    }

    [Serializable]
    public struct FigmaImporterLogReference
    {
        public FigmaImporterLog log;
        public Object target;

        public FigmaImporterLogReference(FigmaImporterLog log, Object target)
        {
            this.log = log;
            this.target = target;
        }
    }

    [Serializable]
    public struct FigmaImporterLog
    {
        public FigmaImporterLogType type;
        public string message;

        public FigmaImporterLog(FigmaImporterLogType type, string message)
        {
            this.type = type;
            this.message = message;
        }
    }

    [Serializable]
    public enum FigmaImporterLogType
    {
        Info,
        Warning,
        Error
    }
}