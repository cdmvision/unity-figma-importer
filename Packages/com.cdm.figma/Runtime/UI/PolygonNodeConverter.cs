namespace Cdm.Figma.UI
{
    public class PolygonNodeConverter : NodeConverter<PolygonNode>
    {
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert(parentObject, (PolygonNode) node, args);
        }
    }
}