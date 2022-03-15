namespace Cdm.Figma.UI
{
    public class RectangleNodeConverter : NodeConverter<RectangleNode>
    {
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert(parentObject, (RectangleNode) node, args);
        }
    }
}