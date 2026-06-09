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

            var propertyReference = node.componentPropertyReferences?.mainComponent;
            if (!string.IsNullOrEmpty(propertyReference))
            {
                if (args.componentPropertyAssignments.TryGetValue(propertyReference!, out var componentNode))
                {
                    FigmaNode componentInstance = null;

                    using (args.OverrideNode(node))
                    {
                        componentInstance = frameNodeConverter.Convert(parentObject, componentNode, args);
                        componentInstance.referenceNode = node;
                    }

                    instanceNode.rectTransform.CopyTo(componentInstance.rectTransform);
                    args.importer.DestroyFigmaNode(instanceNode);
                    return componentInstance;
                }

                if (args.isImportingComponentSet)
                {
                    args.importer.LogWarning($"Instance node {node} instance swap property reference could not found:" +
                                             $" {propertyReference}", instanceNode);
                }

                /*Debug.LogWarning(
                    $"It is ok for single component." +
                    $"\nInstance node {node} instance swap property reference could not found: {propertyReference}");*/
            }

            return instanceNode;
        }
    }
}