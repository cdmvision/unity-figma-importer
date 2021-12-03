using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class GroupNodeConverter : NodeConverter<GroupNode>
    {
        public override NodeData Convert(Node node, NodeConvertArgs args)
        {
            return XmlFactory.NewElement<VisualElement>(node, args);
        }
    }
}