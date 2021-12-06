using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class GroupNodeConverter : NodeConverter<GroupNode>
    {
        public override NodeElement Convert(Node node, NodeConvertArgs args)
        {
            var groupNode = (GroupNode) node;
            
            var element = NodeElement.New<VisualElement>(groupNode, args);
            
            var children = groupNode.children;
            if (children != null)
            {
                foreach (var child in children)
                {
                    if (args.importer.TryConvertNode(child, args, out var childElement))
                    {
                        element.AddChild(childElement);       
                    }
                }
            }
            
            return NodeElement.New<VisualElement>(node, args);
        }
    }
}