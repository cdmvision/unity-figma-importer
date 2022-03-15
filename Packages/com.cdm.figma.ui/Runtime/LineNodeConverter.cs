namespace Cdm.Figma.UI
{
    public class LineNodeConverter : NodeConverter<LineNode>
    {
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert(parentObject, (LineNode) node, args);
        }
    }
}