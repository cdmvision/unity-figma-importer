using System.Xml.Linq;

namespace Cdm.Figma.UIToolkit
{
    public class LineNodeConverter : NodeConverter<LineNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((LineNode) node, args);
        }
    }
}