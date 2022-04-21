namespace Cdm.Figma.UIToolkit
{
    public abstract class NodeConverter : INodeConverter
    {
        public abstract bool CanConvert(Node node, NodeConvertArgs args);
        public abstract NodeElement Convert(NodeElement parentElement, Node node, NodeConvertArgs args);
    }

    public abstract class NodeConverter<TNodeType> : NodeConverter where TNodeType : Node
    {
        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            return node.GetType() == typeof(TNodeType);
        }
    }
}