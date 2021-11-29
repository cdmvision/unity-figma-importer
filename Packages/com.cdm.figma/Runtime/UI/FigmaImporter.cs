using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Cdm.Figma.UI
{
    [CreateAssetMenu(fileName = nameof(UI) + "-" + nameof(FigmaImporter),
        menuName = AssetMenuRoot + "Figma Importer", order = 0)]
    public class FigmaImporter : Figma.FigmaImporter
    {
        public new const string AssetMenuRoot = Figma.FigmaImporter.AssetMenuRoot + "UI/";
        
        [SerializeField]
        private string _typePrefix = "@type:";

        public string typePrefix
        {
            get => _typePrefix;
            set => _typePrefix = value;
        }
        
        [SerializeField]
        private string _bindingPrefix = "@id:";

        public string bindingPrefix
        {
            get => _bindingPrefix;
            set => _bindingPrefix = value;
        }
        
        [SerializeField]
        private string _localizationPrefix = "@lang:";

        public string localizationPrefix
        {
            get => _localizationPrefix;
            set => _localizationPrefix = value;
        }
        
        [SerializeField]
        private string _assetsPath = "Resources/Figma/Prefabs";

        public string assetsPath => _assetsPath;
        
        [SerializeField]
        private List<NodeConverter> _nodeConverters = new List<NodeConverter>();

        public List<NodeConverter> nodeConverters => _nodeConverters;
        
        public override Task ImportFileAsync(FigmaFile file, FigmaImportOptions options = null)
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
        }
        
        internal bool TryConvertNode(Node node, NodeConvertArgs args, out NodeObject nodeObject)
        {
            // Try with component converters first.
            /*var componentConverter = componentConverters.FirstOrDefault(c => c.CanConvert(node, args));
            if (componentConverter != null)
            {
                element = componentConverter.Convert(node, args);
                return true;
            }*/
            
            // Try with node converters.
            var nodeConverter = nodeConverters.FirstOrDefault(c => c.CanConvert(node, args));
            if (nodeConverter != null)
            {
                nodeObject = nodeConverter.Convert(node, args);
                return true;
            }
            
            nodeObject = null;
            return false;
        }
    }
}