using System.Xml;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    public abstract class NodeConverter : ScriptableObject, INodeConverter
    {
        protected const string AssetMenuRoot = FigmaImporter.AssetMenuRoot + "Converters/";
        
        public abstract bool CanConvert(FigmaImporter importer, FigmaFile file, Node node);
        public abstract XmlElement Convert(FigmaImporter importer, FigmaFile file, Node node);
    }

    public abstract class NodeConverter<TNodeType> : NodeConverter where TNodeType : Node
    {
        public override bool CanConvert(FigmaImporter importer, FigmaFile file, Node node)
        {
            return node.GetType() == typeof(TNodeType);
        }
    }
}