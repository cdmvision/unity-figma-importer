namespace Cdm.Figma.UIToolkit
{
    public class StarNodeConverter : NodeConverter<StarNode>
    {
        public override NodeData Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((StarNode) node, args);
        }
    }
}