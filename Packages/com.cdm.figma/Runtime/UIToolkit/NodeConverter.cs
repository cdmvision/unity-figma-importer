using System.Xml.Linq;

namespace Cdm.Figma.UIToolkit
{
    public abstract class NodeConverter : INodeConverter
    {
        protected const string AssetMenuRoot = FigmaImporter.AssetMenuRoot;
        
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