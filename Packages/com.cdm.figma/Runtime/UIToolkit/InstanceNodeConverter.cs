using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class InstanceNodeConverter : NodeConverter<InstanceNode>
    {
        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            if (!base.CanConvert(node, args))
                return false;
            
            var instanceNode = (InstanceNode) node;
            if (instanceNode.mainComponent == null)
                return false;

            if (instanceNode.mainComponent.componentSet != null)
                return false;

            return true;
        }

        public override NodeElement Convert(NodeElement parentElement, Node node, NodeConvertArgs args)
        {
            var instanceNode = (InstanceNode) node;

            var element = NodeElement.New<VisualElement>(instanceNode, args);
            
            // TODO: Call FrameNodeConverter
            
            var children = instanceNode.children;
            if (children != null)
            {
                foreach (var child in children)
                {
                    if (args.importer.TryConvertNode(element, child, args, out var childElement))
                    {
                        if (childElement != element)
                        {
                            element.AddChild(childElement);    
                        }
                    }
                }
            }

            return element;
        }
    }
}