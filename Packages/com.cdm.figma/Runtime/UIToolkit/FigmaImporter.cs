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

        public override async Task ImportFileAsync(FigmaFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var assetsDirectory = Path.Combine("Assets", assetsPath);
            Directory.CreateDirectory(assetsDirectory);

            var pages = file.document.children;

            foreach (var page in pages)
            {
                var pageXml = new XDocument();
                // TODO: xml definition
                // TODO: generate page xml.

                var nodes = page.children;
                foreach (var node in nodes)
                {
                    if (TryConvertNode(file, node, out var element))
                    {
                        pageXml.Add(element);
                    }
                    else
                    {
                        Debug.LogWarning($"Node [{node.id}, {node.name}] could not be converted.");
                    }
                }

                var documentPath = Path.Combine(assetsDirectory, $"{page.id}.uxml");
                using var fileStream = System.IO.File.Create(documentPath);
                using var xmlWriter = new XmlTextWriter(fileStream, Encoding.UTF8);
                await pageXml.SaveAsync(xmlWriter, CancellationToken.None);
                
                Debug.Log($"UI document has been saved to: {documentPath}");
            }
        }

        internal bool TryConvertNode(FigmaFile file, Node node, out XmlElement element)
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