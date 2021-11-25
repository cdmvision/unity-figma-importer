using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(UIToolkit) + "-" + nameof(FigmaImporter), 
        menuName = AssetMenuRoot + "Figma Importer", order = 0)]
    public class FigmaImporter : Figma.FigmaImporter
    {
        public new const string AssetMenuRoot = Figma.FigmaImporter.AssetMenuRoot + "UI Toolkit/";

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
        private string _assetsPath = "Resources/Figma/Documents";

        public string assetsPath => _assetsPath;

        [SerializeField]
        private List<NodeConverter> _nodeConverters = new List<NodeConverter>();

        public List<NodeConverter> nodeConverters => _nodeConverters;
        
        [SerializeField]
        private List<ComponentConverter> _componentConverters = new List<ComponentConverter>();

        public List<ComponentConverter> componentConverters => _componentConverters;

        private List<ComponentSetNode> _componentSets = new List<ComponentSetNode>();

        public IReadOnlyList<ComponentSetNode> componentSets => _componentSets;
        
        public override async Task ImportFileAsync(FigmaFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var assetsDirectory = Path.Combine("Assets", assetsPath);
            Directory.CreateDirectory(assetsDirectory);

            // TODO: collect component set!
            // _componentSets
            
            var pages = file.document.children;

            foreach (var page in pages)
            {
                var xml = new XDocument();
                var root = new XElement("UXML");
                root.SetAttributeValue("xmlns", "UnityEngine.UIElements");
                root.SetAttributeValue("xmlns", "UnityEditor.UIElements");
                root.SetAttributeValue("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                root.SetAttributeValue("engine", "UnityEngine.UIElements");
                root.SetAttributeValue("noNamespaceSchemaLocation", "../../UIElementsSchema/UIElements.xsd");
                root.Add();
                xml.Add(root);

                // TODO: xml definition
                // TODO: generate page xml.

                var nodes = page.children;
                foreach (var node in nodes)
                {
                    if (TryConvertNode(file, node, out var element))
                    {
                        xml.Add(element);
                    }
                }

                await Task.Run(() =>
                {
                    var documentPath = Path.Combine(assetsDirectory, $"{page.name}.uxml");
                    using var fileStream = System.IO.File.Create(documentPath);
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

        internal bool TryConvertNode(FigmaFile file, Node node, out XElement element)
        {
            // Try with component converters first.
            var componentConverter = componentConverters.FirstOrDefault(c => c.CanConvert(this, file, node));
            if (componentConverter != null)
            {
                element = componentConverter.Convert(this, file, node);
                return true;
            }
            
            // Try with node converters.
            var nodeConverter = nodeConverters.FirstOrDefault(c => c.CanConvert(this, file, node));
            if (nodeConverter != null)
            {
                element = nodeConverter.Convert(this, file, node);
                return true;
            }
            
            element = null;
            return false;
        }
    }
}