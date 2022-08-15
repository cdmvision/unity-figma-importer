using UnityEngine;

namespace Cdm.Figma.UI
{
    public class GroupNodeConverter : NodeConverter<GroupNode>
    {
        protected override NodeObject Convert(NodeObject parentObject, GroupNode groupNode, NodeConvertArgs args)
        {
            var nodeObject = NodeObject.Create<NodeObject>(groupNode, args);

            nodeObject.rectTransform.anchorMin = new Vector2(0, 0);
            nodeObject.rectTransform.anchorMax = new Vector2(1, 1);
            nodeObject.rectTransform.offsetMin = new Vector2(0, 0);
            nodeObject.rectTransform.offsetMax = new Vector2(0, 0);

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

            BuildChildren(groupNode, nodeObject, args);

            return nodeObject;
        }

        private static void BuildChildren(GroupNode currentNode, NodeObject nodeObject, NodeConvertArgs args)
        {
            var children = currentNode.GetChildren();
            if (children != null)
            {
                foreach (var child in children)
                {
                    if (args.importer.TryConvertNode(nodeObject, child, args, out var childObject))
                    {
                        if (childObject != nodeObject)
                        {
                            childObject.rectTransform.SetParent(nodeObject.rectTransform, false);
                            childObject.AdjustPosition(currentNode.size);
                        }
                    }
                }
            }
        }
    }
}