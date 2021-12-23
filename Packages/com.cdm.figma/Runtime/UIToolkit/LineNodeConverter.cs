namespace Cdm.Figma.UIToolkit
{
    public class LineNodeConverter : NodeConverter<LineNode>
    {
        public override NodeElement Convert(NodeElement parentElement, Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert(parentElement, (LineNode) node, args);
        }
    }
}