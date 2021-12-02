using System.Xml.Linq;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class GroupNodeConverter : NodeConverter<GroupNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            return XmlFactory.NewElement<VisualElement>(node, args);
        }
    }
}