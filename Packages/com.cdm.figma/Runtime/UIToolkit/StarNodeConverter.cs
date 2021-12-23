namespace Cdm.Figma.UIToolkit
{
    public class StarNodeConverter : NodeConverter<StarNode>
    {
        public override NodeElement Convert(NodeElement parentElement, Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert(parentElement, (StarNode) node, args);
        }
    }
}