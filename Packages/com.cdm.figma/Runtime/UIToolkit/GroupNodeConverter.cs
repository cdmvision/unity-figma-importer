using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class GroupNodeConverter : NodeConverter<GroupNode>
    {
        public override NodeElement Convert(Node node, NodeConvertArgs args)
        {
            return NodeElement.New<VisualElement>(node, args);
        }
    }
}