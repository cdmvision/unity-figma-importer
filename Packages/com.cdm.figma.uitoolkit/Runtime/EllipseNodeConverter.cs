namespace Cdm.Figma.UIToolkit
{
    public class EllipseNodeConverter: NodeConverter<EllipseNode>
    {
        public override NodeElement Convert(NodeElement parentElement, Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert(parentElement, (EllipseNode) node, args);
        }
    }
}