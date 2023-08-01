using UnityEngine;

namespace Cdm.Figma.UI
{
    public class GroupNodeConverter : NodeConverter<GroupNode>
    {
        protected override FigmaNode Convert(FigmaNode parentObject, GroupNode groupNode, NodeConvertArgs args)
        {
            var figmaNode = args.importer.CreateFigmaNode<FigmaNode>(groupNode);
            figmaNode.rectTransform.anchorMin = new Vector2(0, 0);
            figmaNode.rectTransform.anchorMax = new Vector2(1, 1);
            figmaNode.rectTransform.offsetMin = new Vector2(0, 0);
            figmaNode.rectTransform.offsetMax = new Vector2(0, 0);

            Node parentNode = null;
            groupNode.TraverseUp(n =>
            {
                if (n.type != NodeType.Group)
                {
                    parentNode = n;
                    return false;
                }

                return true;
            });

            if (parentNode is INodeTransform parentTransform)
            {
                groupNode.relativeTransform = parentTransform.relativeTransform;
                groupNode.size = parentTransform.size;
            }

            BuildChildren(groupNode, figmaNode, args);

            if (groupNode.isMask)
            {
                args.importer.LogWarning("Group node with mask is not supported.", figmaNode);
            }

            return figmaNode;
        }

        private static void BuildChildren(GroupNode currentNode, FigmaNode nodeObject, NodeConvertArgs args)
        {
            var children = currentNode.GetChildren();
            if (children != null)
            {
                for (var i = 0; i < children.Length; i++)
                {
                    if (children[i].IsIgnored())
                        continue;

                    Node overrideNode = null;

                    var overrideNodeChildren = args.overrideNode?.GetChildren();
                    if (overrideNodeChildren != null && overrideNodeChildren.Length == children.Length)
                    {
                        overrideNode = overrideNodeChildren[i];
                    }

                    using (args.OverrideNode(overrideNode))
                    {
                        if (args.importer.TryConvertNode(nodeObject, children[i], args, out var childObject))
                        {
                            if (childObject != nodeObject)
                            {
                                childObject.rectTransform.SetParent(nodeObject.rectTransform, false);
                                childObject.AdjustRectTransform(currentNode.size);

                                // Transform importing is disabled due to a bug right now.
                                // Add transform style after all changes made on rect transform.
                                //childObject.styles.Add(TransformStyle.GetTransformStyle(childObject.rectTransform));
                            }
                        }
                    }
                }
            }
        }
    }
}