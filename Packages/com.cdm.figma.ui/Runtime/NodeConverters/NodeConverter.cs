namespace Cdm.Figma.UI
{
    public abstract class NodeConverter : INodeConverter
    {
        public abstract bool CanConvert(Node node, NodeConvertArgs args);
        public abstract FigmaNode Convert(FigmaNode parentObject, Node node, NodeConvertArgs args);
    }

    public abstract class NodeConverter<TNodeType> : NodeConverter where TNodeType : Node
    {
        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            return node.GetType() == typeof(TNodeType);
        }

        public override FigmaNode Convert(FigmaNode parentObject, Node node, NodeConvertArgs args)
        {
            return Convert(parentObject, (TNodeType) node, args);
        }
        
        protected abstract FigmaNode Convert(FigmaNode parentObject, TNodeType node, NodeConvertArgs args);
    }
}