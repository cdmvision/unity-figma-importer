namespace Cdm.Figma.UI
{
    public abstract class NodeConverter : INodeConverter
    {
        public abstract bool CanConvert(Node node, NodeConvertArgs args);
        public abstract NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args);
    }

    public abstract class NodeConverter<TNodeType> : NodeConverter where TNodeType : Node
    {
        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            return node.GetType() == typeof(TNodeType);
        }

        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return Convert(parentObject, (TNodeType) node, args);
        }
        
        protected abstract NodeObject Convert(NodeObject parentObject, TNodeType node, NodeConvertArgs args);
    }
}