namespace Cdm.Figma.UI
{
    public class RegularPolygonNodeConverter : NodeConverter<PolygonNode>
    {
        public override NodeObject Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((PolygonNode) node, args);
        }
    }
}