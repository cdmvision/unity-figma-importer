using System.Xml.Linq;

namespace Cdm.Figma.UIToolkit
{
    public class StarNodeConverter : NodeConverter<StarNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            return VectorNodeConverter.Convert((StarNode) node, args);
        }
    }
}