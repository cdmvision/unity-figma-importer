namespace Cdm.Figma.UIToolkit
{
    public class PolygonNodeConverter : NodeConverter<PolygonNode>
    {
        public override NodeData Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((PolygonNode) node, args);
        }
    }
}