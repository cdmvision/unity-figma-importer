namespace Cdm.Figma.UI
{
    public class RectangleNodeConverter : NodeConverter<RectangleNode>
    {
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((RectangleNode) node, args);
        }
    }
}