namespace Cdm.Figma.UI
{
    public class EllipseNodeConverter : NodeConverter<StarNode>
    {
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert(parentObject, (StarNode) node, args);
        }
    }
}