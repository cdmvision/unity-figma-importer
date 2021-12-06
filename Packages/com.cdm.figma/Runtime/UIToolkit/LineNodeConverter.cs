namespace Cdm.Figma.UIToolkit
{
    public class LineNodeConverter : NodeConverter<LineNode>
    {
        public override NodeElement Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((LineNode) node, args);
        }
    }
}