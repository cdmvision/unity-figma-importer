using System.Xml.Linq;

namespace Cdm.Figma.UIToolkit.UIToolkit
{
    public class EllipseNodeConverter: NodeConverter<EllipseNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((EllipseNode) node, args);
        }
    }
}