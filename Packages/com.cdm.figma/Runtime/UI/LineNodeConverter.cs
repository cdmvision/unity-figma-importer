namespace Cdm.Figma.UI
{
    public class LineNodeConverter : NodeConverter<LineNode>
    {
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((LineNode) node, args);
        }
    }
}