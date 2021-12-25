using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cdm.Figma.UI
{
    public class FigmaImporter : IFigmaImporter
    {
        private readonly HashSet<NodeConverter> _nodeConverters = new HashSet<NodeConverter>();

        public ISet<NodeConverter> nodeConverters => _nodeConverters;

        private readonly HashSet<ComponentConverter> _componentConverters = new HashSet<ComponentConverter>();

        public ISet<ComponentConverter> componentConverters => _componentConverters;
        
        public Figma.FigmaFile CreateFile(string fileId, string fileContent, byte[] thumbnailData = null)
        {
            throw new NotImplementedException();
        }

        public Task ImportFileAsync(Figma.FigmaFile file)
        {
            throw new NotImplementedException();
        }
        
        internal bool TryConvertNode(Node node, NodeConvertArgs args, out NodeObject nodeObject)
        {
            // Try with component converters first.
            var componentConverter = componentConverters.FirstOrDefault(c => c.CanConvert(node, args));
            if (componentConverter != null)
            {
                nodeObject = componentConverter.Convert(node, args);
                return true;
            }

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
        
        /*internal bool TryConvertNode(Node node, NodeConvertArgs args, out NodeObject nodeObject)
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
                nodeObject = nodeConverter.Convert(node, args);
                return true;
            }
            
            nodeObject = null;
            return false;
        }*/
    }
}