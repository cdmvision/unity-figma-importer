using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit.UIToolkit
{
    public class InstanceNodeConverter : NodeConverter<InstanceNode>
    {
        public override NodeData Convert(Node node, NodeConvertArgs args)
        {
            var instanceNode = (InstanceNode) node;

            var data = XmlFactory.NewElement<VisualElement>(node, args);
            
            var children = instanceNode.children;
            if (children != null)
            {
                foreach (var child in children)
                {
                    if (args.importer.TryConvertNode(child, args, out var childElement))
                    {
                        data.element.Add(childElement);       
                    }
                }
            }

            return data;
        }
    }
}