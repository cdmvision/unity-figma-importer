using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit.UIToolkit
{
    public class InstanceNodeConverter : NodeConverter<InstanceNode>
    {
        public override NodeElement Convert(Node node, NodeConvertArgs args)
        {
            var instanceNode = (InstanceNode) node;

            var element = NodeElement.New<VisualElement>(instanceNode, args);
            
            var children = instanceNode.children;
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

            return element;
        }
    }
}