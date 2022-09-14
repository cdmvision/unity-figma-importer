using Cdm.Figma.UI.Utils;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class InstanceNodeConverter : NodeConverter<InstanceNode>
    {
        protected override FigmaNode Convert(FigmaNode parentObject, InstanceNode node, NodeConvertArgs args)
        {
            var frameNodeConverter = new FrameNodeConverter();

            // Use instance node's transform.
            var instanceNode = frameNodeConverter.Convert(parentObject, node, args);

            var propertyReference = node.componentProperties?.references?.mainComponent;
            if (!string.IsNullOrEmpty(propertyReference))
            {
                if (args.componentPropertyAssignments.TryGetValue(propertyReference, out var componentNode))
                {
                    var componentInstance = frameNodeConverter.Convert(parentObject, componentNode, args);
                    componentInstance.referenceNode = node;
                    
                    instanceNode.rectTransform.CopyTo(componentInstance.rectTransform);

                    args.importer.DestroyFigmaNode(instanceNode);
                    return componentInstance;
                }

                Debug.LogWarning(
                    $"Instance node {node} instance swap property reference could not found: {propertyReference}");
            }

            return instanceNode;
        }
    }
}