using Cdm.Figma.UI.Utils;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class InstanceNodeConverter : NodeConverter<InstanceNode>
    {
        /// <summary>
        /// Converts an instance node if <see cref="ComponentConverter"/>s fail.
        /// </summary>
        public override bool CanConvert(Node node, NodeConvertArgs args)
        {
            if (!base.CanConvert(node, args))
                return false;

            var instanceNode = (InstanceNode)node;

            // We can convert even if main component is missing. We don't need to take main component into account.
            if (instanceNode.mainComponent?.componentSet != null)
                return false;

            return true;
        }

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