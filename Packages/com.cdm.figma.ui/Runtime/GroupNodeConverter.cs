using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class GroupNodeConverter : GroupNodeConverter<GroupNode>
    {
    }

    public abstract class GroupNodeConverter<TNode> : NodeConverter<TNode> where TNode : GroupNode
    {
        protected override NodeObject Convert(NodeObject parentObject, TNode groupNode, NodeConvertArgs args)
        {
            var nodeObject = NodeObject.NewNodeObject(groupNode, args);

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
            var children = currentNode.children;
            if (children != null)
            {
                for (var child = 0; child < children.Length; child++)
                {
                    if (args.importer.TryConvertNode(nodeObject, children[child], args, out var childObject))
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