using System.Xml.Linq;

namespace Cdm.Figma.UIToolkit
{
    public class PolygonNodeConverter : NodeConverter<PolygonNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((PolygonNode) node, args);
        }
    }
}