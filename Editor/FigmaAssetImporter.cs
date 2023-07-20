using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cdm.Figma.Editor;
using Cdm.Figma.UI.Editor.Utils;
using Cdm.Figma.Utils;
using TMPro;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cdm.Figma.UI.Editor
{
    [ScriptedImporter(1, DefaultExtension, ImportQueueOffset)]
    public class FigmaAssetImporter : FigmaAssetImporterBase
    {
        private const string TextMeshProSettingsAssetPath = "Assets/TextMesh Pro/Resources/TMP Settings.asset";
        
        [SerializeField]
        private FontSource[] _fonts;

        /// <summary>
        /// Gets the font assets.
        /// </summary>
        public FontSource[] fonts => _fonts;

        [SerializeField]
        private TMP_FontAsset _fallbackFont;

        /// <summary>
        /// Gets or sets the fallback font that is used when a font mapping does not found.
        /// </summary>
        public TMP_FontAsset fallbackFont
        {
            get => _fallbackFont;
            set => _fallbackFont = value;
        }

        [SerializeField]
        private FigmaPage[] _pageReferences;

        public FigmaPage[] pageReferences
        {
            get => _pageReferences;
        }

        [SerializeField]
        private int _layer;

        /// <inheritdoc cref="FigmaImporter.layer"/>
        public int layer
        {
            get => _layer;
            set => _layer = value;
        }

        [SerializeField]
        private float _pixelsPerUnit = 100f;

        /// <inheritdoc cref="SpriteGenerateOptions.pixelsPerUnit"/>
        public float pixelsPerUnit
        {
            get => _pixelsPerUnit;
            set => _pixelsPerUnit = value;
        }

        [SerializeField, Min(0.001f)]
        private float _scaleFactor = 1f;

        /// <inheritdoc cref="SpriteGenerateOptions.scaleFactor"/>
        public float scaleFactor
        {
            get => _scaleFactor;
            set => _scaleFactor = Mathf.Max(0.001f, value);
        }

        [SerializeField]
        private ushort _gradientResolution = 128;

        /// <inheritdoc cref="SpriteGenerateOptions.gradientResolution"/>
        public ushort gradientResolution
        {
            get => _gradientResolution;
            set => _gradientResolution = value;
        }

        [SerializeField]
        private int _minTextureSize = 64;

        /// <inheritdoc cref="SpriteGenerateOptions.minTextureSize"/>
        public int minTextureSize
        {
            get => _minTextureSize;
            set => _minTextureSize = value;
        }

        [SerializeField]
        private int _maxTextureSize = 1024;

        /// <inheritdoc cref="SpriteGenerateOptions.maxTextureSize"/>
        public int maxTextureSize
        {
            get => _maxTextureSize;
            set => _maxTextureSize = value;
        }

        [SerializeField]
        private TextureWrapMode _wrapMode = TextureWrapMode.Clamp;

        /// <inheritdoc cref="SpriteGenerateOptions.wrapMode"/>
        public TextureWrapMode wrapMode
        {
            get => _wrapMode;
            set => _wrapMode = value;
        }

        [SerializeField]
        private FilterMode _filterMode = FilterMode.Bilinear;

        /// <inheritdoc cref="SpriteGenerateOptions.filterMode"/>
        public FilterMode filterMode
        {
            get => _filterMode;
            set => _filterMode = value;
        }

        [SerializeField]
        private int _sampleCount = 4;
        
        /// <inheritdoc cref="SpriteGenerateOptions.sampleCount"/>
        public int sampleCount
        {
            get => _sampleCount;
            set => _sampleCount = value;
        }

        // Draw the sprite again, but clear with the texture rendered in the previous step,
        // this will make the bilinear filter to interpolate the colors with values different
        // than "transparent black", which causes black-ish outlines around the shape.
        
        [Tooltip("When true, expand the edges to avoid a dark banding effect caused by filtering." + 
                 "This is slower to render and uses more graphics memory.")]
        [SerializeField]
        private bool _expandEdges = false;
        
        /// <inheritdoc cref="SpriteGenerateOptions.expandEdges"/>
        public bool expandEdges
        {
            get => _expandEdges;
            set => _expandEdges = value;
        }

        [SerializeField]
        private bool _markExternalAssetAsDependency = true;
        
        public bool markExternalAssetAsDependency
        {
            get => _markExternalAssetAsDependency;
            set => _markExternalAssetAsDependency = value;
        }

        [Tooltip("If you disable this, you might see bunch of 'Identifier uniqueness violation' warnings on the console.")]
        [SerializeField]
        private bool _generateUniqueNodeName = false;

        /// <summary>
        /// Generates unique name for each <see cref="GameObject"/> node by appending its
        /// Figma node <see cref="Node.id"/>.
        /// </summary>
        /// <remarks>
        /// If you set this to <c>false</c>, you might see bunch of 'Identifier uniqueness violation' warnings
        /// on the console.
        /// </remarks>
        public bool generateUniqueNodeName
        {
            get => _generateUniqueNodeName;
            set => _generateUniqueNodeName = value;
        }

        [SerializeField]
        [SerializedType(typeof(ILocalizationConverter))]
        private string _localizationConverter;

        [SerializeField]
        [SerializedType(typeof(IEffectConverter))]
        private List<string> _effectConverters = new List<string>();

        [SerializeField]
        private List<SerializableAssetBinding> _assetBindings = new List<SerializableAssetBinding>();

        public override void OnImportAsset(AssetImportContext ctx)
        {
            base.OnImportAsset(ctx);

            if (importSettingsMissing)
            {
                layer = LayerMask.NameToLayer("UI");
            }
        }

        private static string[] GatherDependenciesFromSourceFile(string path)
        {
            return new[] { TextMeshProSettingsAssetPath };
        }

        protected override void OnAssetImporting(AssetImportContext ctx, IFigmaImporter figmaImporter,
            FigmaFile figmaFile)
        {
            base.OnAssetImporting(ctx, figmaImporter, figmaFile);
            
            UpdateFonts((FigmaImporter)figmaImporter, figmaFile);
            ForceLoadTextMeshProSettings();
        }

        protected override void OnAssetImported(AssetImportContext ctx, IFigmaImporter figmaImporter,
            FigmaFile figmaFile, Figma.FigmaDesign figmaDesign)
        {
            base.OnAssetImported(ctx, figmaImporter, figmaFile, figmaDesign);

            // Add imported page game objects to the asset.
            var design = (FigmaDesign)figmaDesign;

            // Add figma nodes.
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(FigmaIconFlatPath);

            // Detach pages from the document to be able to use as single page instead of full document.
            design.document.transform.DetachChildren();

            // Add pages.
            foreach (var page in design.document.pages)
            {
                ctx.AddObjectToAsset($"{page.nodeId}", page.gameObject, icon);

                if (generateUniqueNodeName)
                {
                    GenerateUniqueName(page);
                }
            }

            var importer = (FigmaImporter)figmaImporter;

            // Add generated objects to the asset.
            foreach (var generatedAsset in importer.generatedAssets)
            {
                if (generatedAsset.Value.hideFlags.HasFlag(HideFlags.DontSaveInBuild) ||
                    generatedAsset.Value.hideFlags.HasFlag(HideFlags.DontSaveInEditor) ||
                    generatedAsset.Value.hideFlags.HasFlag(HideFlags.DontUnloadUnusedAsset) ||
                    generatedAsset.Value.hideFlags.HasFlag(HideFlags.DontSave) ||
                    generatedAsset.Value.hideFlags.HasFlag(HideFlags.HideAndDontSave))
                {
                    Debug.LogWarning(
                        $"An asset '{generatedAsset.Value.name}' marked with {generatedAsset.Value.hideFlags} " +
                        "might cause a build error. Did you forget to mark with HideFlags.None?", generatedAsset.Value);
                }

                ctx.AddObjectToAsset(generatedAsset.Key, generatedAsset.Value);
            }

            // Register dependency assets.
            if (markExternalAssetAsDependency)
            {
                foreach (var dependencyAsset in importer.dependencyAssets)
                {
                    if (dependencyAsset.Value.hideFlags.HasFlag(HideFlags.DontSaveInBuild) ||
                        dependencyAsset.Value.hideFlags.HasFlag(HideFlags.DontSaveInEditor) ||
                        dependencyAsset.Value.hideFlags.HasFlag(HideFlags.DontUnloadUnusedAsset) ||
                        dependencyAsset.Value.hideFlags.HasFlag(HideFlags.DontSave) ||
                        dependencyAsset.Value.hideFlags.HasFlag(HideFlags.HideAndDontSave))
                    {
                        Debug.LogWarning(
                            $"A dependency asset '{dependencyAsset.Value.name}' " +
                            $"marked with {dependencyAsset.Value.hideFlags} might cause an error.", dependencyAsset.Value);
                    }

                    ctx.DependsOnSourceAsset(AssetDatabase.GetAssetPath(dependencyAsset.Value));
                }
            }
            
            UpdatePageReferences(design);
        }

        protected override IFigmaImporter GetFigmaImporter(AssetImportContext ctx)
        {
            var spriteOptions = SpriteGenerateOptions.GetDefault();
            spriteOptions.pixelsPerUnit = pixelsPerUnit;
            spriteOptions.scaleFactor = scaleFactor;
            spriteOptions.gradientResolution = gradientResolution;
            spriteOptions.minTextureSize = minTextureSize;
            spriteOptions.maxTextureSize = maxTextureSize;
            spriteOptions.wrapMode = wrapMode;
            spriteOptions.filterMode = filterMode;
            spriteOptions.sampleCount = sampleCount;
            spriteOptions.expandEdges = expandEdges;

            var figmaImporter = new FigmaImporter()
            {
                layer = layer,
                failOnError = false,
                spriteOptions = spriteOptions
            };

            SetLocalizationConverter(figmaImporter);
            AddEffectConverters(figmaImporter);

            // Prioritize custom converters.
            SearchAndAddFigmaComponentBehaviours(figmaImporter);
            SearchAndAddFigmaNodeBehaviours(figmaImporter);

            SearchAndAddComponentConverters(figmaImporter);
            SearchAndAddNodeConverters(figmaImporter);

            SetAssetBindings(ctx, figmaImporter);

            figmaImporter.AddDefaultNodeConverters();
            figmaImporter.AddDefaultComponentConverters();

            return figmaImporter;
        }
        
        private static void GenerateUniqueName(FigmaPage page)
        {
            var nodes = page.GetComponentsInChildren<FigmaNode>(true);
            foreach (var node in nodes)
            {
                node.name += $" ({node.nodeId})";
            }

            // Do not append id on the page name.
            page.name = page.nodeName;
        }
        
        private void SetAssetBindings(AssetImportContext ctx, FigmaImporter figmaImporter)
        {
            figmaImporter.assetBindings = new HashSet<FigmaAssetBinding>();

            foreach (var binding in _assetBindings)
            {
                var type = Type.GetType(binding.type);
                if (type == null)
                    continue;

                var assetBinding = new FigmaAssetBinding(type);
                assetBinding.memberBindings = new HashSet<FigmaAssetBindingMember>();

                foreach (var memberBinding in binding.bindings)
                {
                    var bindingAttributes = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    var members = type.GetMember(memberBinding.name, bindingAttributes);
                    
                    if (members.Length <= 0)
                        continue;

                    if (memberBinding.asset.isSet &&
                        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(memberBinding.asset, out var guid, out long _))
                    {
                        ctx.DependsOnArtifact(guid);
                    }
                    else
                    {
                        continue;
                    }
                    
                    // We don't care methods, so there should be only one member.
                    assetBinding.memberBindings.Add(new FigmaAssetBindingMember(members[0], memberBinding.asset.asset));
                }

                figmaImporter.assetBindings.Add(assetBinding);
            }
        }

        private void SetLocalizationConverter(FigmaImporter figmaImporter)
        {
            var typeName = _localizationConverter;

            if (string.IsNullOrWhiteSpace(typeName))
                return;

            var localizationConverterType = Type.GetType(typeName);
            if (localizationConverterType != null)
            {
                var localizationConverter = (ILocalizationConverter)Activator.CreateInstance(localizationConverterType);
                if (localizationConverter != null)
                {
                    figmaImporter.localizationConverter = localizationConverter;
                    //Debug.Log($"Localization converter set: {localizationConverter.GetType().FullName}");
                }
                else
                {
                    Debug.LogWarning($"Localization converter could not be set: {typeName}");
                }
            }
            else
            {
                Debug.LogWarning($"Localization converter could not be set: {typeName}");
            }
        }

        private void AddEffectConverters(FigmaImporter figmaImporter)
        {
            foreach (var typeName in _effectConverters)
            {
                if (string.IsNullOrWhiteSpace(typeName))
                    continue;

                var effectConverterType = Type.GetType(typeName);
                if (effectConverterType != null)
                {
                    var effectConverter = (IEffectConverter)Activator.CreateInstance(effectConverterType);
                    if (effectConverter != null)
                    {
                        figmaImporter.effectConverters.Add(effectConverter);
                        //Debug.Log($"Effect converter added: {effectConverter.GetType().FullName}");
                    }
                    else
                    {
                        Debug.LogWarning($"Effect converter could not be added: {typeName}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Effect converter could not be added: {typeName}");
                }
            }
        }

        private void SearchAndAddFigmaNodeBehaviours(FigmaImporter figmaImporter)
        {
            var nodeBehaviours = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(FigmaNodeAttribute))));

            foreach (var type in nodeBehaviours)
            {
                if (typeof(UnityEngine.Component).IsAssignableFrom(type))
                {
                    var figmaNodeAttribute =
                        (FigmaNodeAttribute)Attribute.GetCustomAttribute(type, typeof(FigmaNodeAttribute));

                    if (figmaNodeAttribute.importerExtension != GetAssetExtension())
                        continue;

                    var bindingKey = figmaNodeAttribute.bindingKey;
                    if (!string.IsNullOrEmpty(bindingKey))
                    {
                        //Debug.Log($"{nameof(FigmaNodeBehaviourConverter)} added for bindingKey '{bindingKey}'.");
                        figmaImporter.nodeConverters.Add(new FigmaNodeBehaviourConverter(bindingKey, type));
                    }
                    else
                    {
                        Debug.LogError($"Cannot add {nameof(FigmaNodeBehaviourConverter)}. " +
                                       $"{nameof(FigmaNodeAttribute)} binding key must not be empty.");
                    }
                }
                else
                {
                    Debug.LogError($"Cannot add {nameof(FigmaNodeBehaviourConverter)}. " +
                                   $"Node behaviour must inherit from {typeof(UnityEngine.Component).FullName}");
                }
            }
        }

        private void SearchAndAddFigmaComponentBehaviours(FigmaImporter figmaImporter)
        {
            var componentBehaviours = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(FigmaComponentAttribute))));

            foreach (var type in componentBehaviours)
            {
                if (typeof(UnityEngine.Component).IsAssignableFrom(type))
                {
                    var figmaComponentAttribute =
                        (FigmaComponentAttribute)Attribute.GetCustomAttribute(type, typeof(FigmaComponentAttribute));

                    if (figmaComponentAttribute.importerExtension != GetAssetExtension())
                        continue;

                    var typeId = figmaComponentAttribute.typeId;
                    figmaImporter.componentConverters.Add(new FigmaComponentBehaviourConverter(typeId, type));
                }
                else
                {
                    Debug.LogError($"Cannot add {nameof(FigmaComponentBehaviourConverter)}. " +
                                   $"Component behaviour must inherit from {typeof(UnityEngine.Component).FullName}");
                }
            }
        }

        private void SearchAndAddComponentConverters(FigmaImporter figmaImporter)
        {
            var componentConverters = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(FigmaComponentConverterAttribute))));

            foreach (var type in componentConverters)
            {
                if (typeof(ComponentConverter).IsAssignableFrom(type))
                {
                    var figmaComponentConverterAttribute =
                        (FigmaComponentConverterAttribute)Attribute.GetCustomAttribute(
                            type, typeof(FigmaComponentConverterAttribute));

                    if (figmaComponentConverterAttribute.importerExtension != GetAssetExtension())
                        continue;

                    figmaImporter.componentConverters.Add((ComponentConverter)Activator.CreateInstance(type));
                }
                else
                {
                    Debug.LogError($"Cannot add component converter '{type.FullName}' to {nameof(FigmaImporter)}. " +
                                   $"Component converter must inherit from {typeof(ComponentConverter).FullName}");
                }
            }
        }

        private void SearchAndAddNodeConverters(FigmaImporter figmaImporter)
        {
            var nodeConverters = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(FigmaNodeConverterAttribute))));

            foreach (var type in nodeConverters)
            {
                if (typeof(NodeConverter).IsAssignableFrom(type))
                {
                    var figmaNodeConverterAttribute =
                        (FigmaNodeConverterAttribute)Attribute.GetCustomAttribute(
                            type, typeof(FigmaNodeConverterAttribute));

                    if (figmaNodeConverterAttribute.importerExtension != GetAssetExtension())
                        continue;

                    figmaImporter.nodeConverters.Add((NodeConverter)Activator.CreateInstance(type));
                }
                else
                {
                    Debug.LogError($"Cannot add node converter '{type.FullName}' to {nameof(FigmaImporter)}. " +
                                   $"Node converter must inherit from {typeof(NodeConverter).FullName}");
                }
            }
        }

        private void UpdateFonts(FigmaImporter figmaImporter, FigmaFile file)
        {
            if (importSettingsMissing)
            {
                _fonts = null;
            }

            var usedFonts = file.GetUsedFonts();
            var oldFonts = _fonts;
            _fonts = new FontSource[usedFonts.Length];

            for (var i = 0; i < _fonts.Length; i++)
            {
                _fonts[i] = new FontSource(usedFonts[i], null);

                // Restore previously assigned fonts.
                if (oldFonts != null)
                {
                    var oldFontIndex = Array.FindIndex(oldFonts, x => x.fontName == _fonts[i].fontName);
                    if (oldFontIndex >= 0)
                    {
                        _fonts[i].font = oldFonts[oldFontIndex].font;
                    }
                }
            }

            figmaImporter.fonts.AddRange(_fonts);
            figmaImporter.fallbackFont = fallbackFont;
        }
        
        private static void ForceLoadTextMeshProSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(TextMeshProSettingsAssetPath);
            if (settings != null)
            {
                var type = typeof(TMP_Settings);
                var field = type.GetField("s_Instance", BindingFlags.NonPublic | BindingFlags.Static);
                field?.SetValue(null, settings);
            }
        }

        private void UpdatePageReferences(FigmaDesign figmaDesign)
        {
            _pageReferences = new FigmaPage[pages.Length];

            var figmaPages = figmaDesign.document.pages;
            for (var i = 0; i < _pageReferences.Length; i++)
            {
                var figmaPage = figmaPages.FirstOrDefault(x => x.nodeId == pages[i].id);
                if (figmaPage != null)
                {
                    _pageReferences[i] = figmaPage;
                }
            }
        }
        
        [Serializable]
        internal struct SerializableAssetBinding
        {
            /// <summary>
            /// Type of the class that having asset binding members.
            /// </summary>
            public string type;
            
            /// <summary>
            /// The asset bindings of the members.
            /// </summary>
            public List<SerializableAssetBindingMember> bindings;

            public SerializableAssetBinding(string type)
            {
                this.type = type;
                this.bindings = new List<SerializableAssetBindingMember>();
            }
        }

        [Serializable]
        internal struct SerializableAssetBindingMember
        {
            /// <summary>
            /// The name of the <see cref="MemberInfo"/>.
            /// </summary>
            public string name;
            
            /// <summary>
            /// The asset is being assigned to the <see cref="MemberInfo"/>.
            /// </summary>
            public LazyLoadReference<Object> asset;
        }
    }
}