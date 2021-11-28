using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public interface INodeConverter
    {
        bool CanConvert(Node node, NodeConvertArgs args);
        NodeObject Convert(Node node, NodeConvertArgs args);
    }
    
    public class NodeConvertArgs
    {
        public FigmaImporter importer { get; }
        public FigmaFile file { get; }
        public List<ComponentSetNode> componentSets { get; } = new List<ComponentSetNode>();
        
        /// <inheritdoc cref="FigmaImportOptions.assets"/>
        public IDictionary<string, string> assets { get; set; } = new Dictionary<string, string>();
        
        public NodeConvertArgs(FigmaImporter importer, FigmaFile file)
        {
            this.importer = importer;
            this.file = file;
        }
    }
}