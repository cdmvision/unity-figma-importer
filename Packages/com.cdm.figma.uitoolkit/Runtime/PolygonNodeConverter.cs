namespace Cdm.Figma.UIToolkit
{
    public class PolygonNodeConverter : NodeConverter<PolygonNode>
    {
        public override NodeElement Convert(NodeElement parentElement, Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert(parentElement, (PolygonNode) node, args);
        }
    }
}