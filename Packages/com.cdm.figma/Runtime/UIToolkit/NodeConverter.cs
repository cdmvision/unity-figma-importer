using System.Xml.Linq;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    public abstract class NodeConverter : ScriptableObject, INodeConverter
    {
        protected const string AssetMenuRoot = FigmaImporter.AssetMenuRoot + "Converters/";
        
        public abstract bool CanConvert(Node node, NodeConvertArgs args);
        public abstract XElement Convert(Node node, NodeConvertArgs args);
    }

    public abstract class NodeConverter<TNodeType> : NodeConverter where TNodeType : Node
    {
        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            return node.GetType() == typeof(TNodeType);
        }
    }
}