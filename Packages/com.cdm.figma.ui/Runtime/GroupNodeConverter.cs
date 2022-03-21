using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class GroupNodeConverter : NodeConverter<GroupNode>
    {
        public static NodeObject Convert(NodeObject parentObject, GroupNode groupNode, NodeConvertArgs args)
        {
            var groupNodeObject = NodeObject.NewNodeObject(groupNode, args);

            BuildUIObject(parentObject, groupNodeObject, groupNode);
            BuildChildren(groupNode, groupNodeObject, groupNodeObject, args);

            return groupNodeObject;
        }

        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            return Convert(parentObject, (GroupNode)node, args);
        }

        private static void BuildUIObject(NodeObject parentObject, NodeObject nodeObject, GroupNode groupNode)
        {
            nodeObject.SetTransform(groupNode, TransformType.Relative);
            nodeObject.SetSize(groupNode, TransformType.Relative);

            if (groupNode is INodeTransform parentTransform)
            {
                nodeObject.SetLayoutConstraints(parentTransform.size);    
            }
            
            if (groupNode.layoutMode != LayoutMode.None)
            {
                AddLayoutComponent(groupNode, nodeObject);
                AddPadding(groupNode, nodeObject);
                AddContentSizeFitter(groupNode, nodeObject);
            }
        }

        private static void BuildChildren(GroupNode currentNode, NodeObject currentNodeObject, NodeObject parentObject,
            NodeConvertArgs args)
        {
            GroupNode parentNode = (GroupNode)parentObject.node;
            INodeTransform parentTransform = (INodeTransform)parentNode;
            var children = currentNode.children;
            if (children != null)
            {
                for (var child = 0; child < children.Length; child++)
                {
                    if (args.importer.TryConvertNode(parentObject, children[child], args, out var childObject))
                    {
                        if (currentNode.layoutMode != LayoutMode.None)
                        {
                            childObject.gameObject.AddComponent<LayoutElement>();
                            HandleFillContainer(currentNode.layoutMode, currentNodeObject, childObject);
                        }

                        //HandleConstraints(parentTransform.size, childObject);

                        if (childObject != parentObject)
                        {
                            childObject.rectTransform.SetParent(parentObject.rectTransform, false);
                        }
                    }
                }
            }
        }

        private static void HandleFillContainer(LayoutMode groupNodeLayoutMode, NodeObject groupNodeObject,
            NodeObject childElement)
        {
            INodeLayout childLayout = (INodeLayout)childElement.node;
            INodeTransform childTransform = (INodeTransform)childElement.node;
            if (childLayout.layoutAlign == LayoutAlign.Stretch)
            {
                if (groupNodeLayoutMode == LayoutMode.Horizontal)
                {
                    groupNodeObject.GetComponent<HorizontalLayoutGroup>().childControlHeight = true;
                    childElement.gameObject.GetComponent<LayoutElement>().flexibleHeight = 1;
                }
                else if (groupNodeLayoutMode == LayoutMode.Vertical)
                {
                    groupNodeObject.GetComponent<VerticalLayoutGroup>().childControlWidth = true;
                    childElement.gameObject.GetComponent<LayoutElement>().flexibleWidth = 1;
                }
            }
            else
            {
                if (groupNodeLayoutMode == LayoutMode.Horizontal)
                {
                    groupNodeObject.GetComponent<HorizontalLayoutGroup>().childControlHeight = true;
                    childElement.gameObject.GetComponent<LayoutElement>().preferredHeight = childTransform.size.y;
                }
                else
                {
                    groupNodeObject.GetComponent<VerticalLayoutGroup>().childControlWidth = true;
                    childElement.gameObject.GetComponent<LayoutElement>().preferredWidth = childTransform.size.x;
                }
            }

            if (childLayout.layoutGrow.HasValue && childLayout.layoutGrow != 0)
            {
                if (groupNodeLayoutMode == LayoutMode.Horizontal)
                {
                    groupNodeObject.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
                    childElement.gameObject.GetComponent<LayoutElement>().flexibleWidth = 1;
                }
                else if (groupNodeLayoutMode == LayoutMode.Vertical)
                {
                    groupNodeObject.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
                    childElement.gameObject.GetComponent<LayoutElement>().flexibleHeight = 1;
                }
            }
            else
            {
                if (groupNodeLayoutMode == LayoutMode.Horizontal)
                {
                    groupNodeObject.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
                    childElement.gameObject.GetComponent<LayoutElement>().preferredWidth = childTransform.size.x;
                }
                else
                {
                    groupNodeObject.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
                    childElement.gameObject.GetComponent<LayoutElement>().preferredHeight = childTransform.size.y;
                }
            }
        }

        private static void AddContentSizeFitter(GroupNode groupNode, NodeObject groupNodeObject)
        {
            if (groupNode.primaryAxisSizingMode == AxisSizingMode.Auto ||
                groupNode.counterAxisSizingMode == AxisSizingMode.Auto)
            {
                groupNodeObject.gameObject.AddComponent<ContentSizeFitter>();
            }

            if (groupNode.primaryAxisSizingMode == AxisSizingMode.Auto)
            {
                if (groupNode.layoutMode == LayoutMode.Horizontal)
                {
                    groupNodeObject.gameObject.GetComponent<ContentSizeFitter>().horizontalFit =
                        ContentSizeFitter.FitMode.PreferredSize;
                }
                else
                {
                    groupNodeObject.gameObject.GetComponent<ContentSizeFitter>().verticalFit =
                        ContentSizeFitter.FitMode.PreferredSize;
                }
            }

            if (groupNode.counterAxisSizingMode == AxisSizingMode.Auto)
            {
                if (groupNode.layoutMode == LayoutMode.Horizontal)
                {
                    groupNodeObject.gameObject.GetComponent<ContentSizeFitter>().verticalFit =
                        ContentSizeFitter.FitMode.PreferredSize;
                }
                else
                {
                    groupNodeObject.gameObject.GetComponent<ContentSizeFitter>().horizontalFit =
                        ContentSizeFitter.FitMode.PreferredSize;
                }
            }
        }

        private static void AddPadding(GroupNode groupNode, NodeObject groupNodeObject)
        {
            if (groupNode.layoutMode == LayoutMode.Horizontal)
            {
                groupNodeObject.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset(
                    (int)groupNode.paddingLeft,
                    (int)groupNode.paddingRight, (int)groupNode.paddingTop, (int)groupNode.paddingBottom);
            }
            else
            {
                groupNodeObject.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(
                    (int)groupNode.paddingLeft,
                    (int)groupNode.paddingRight, (int)groupNode.paddingTop, (int)groupNode.paddingBottom);
            }
        }

        private static void AddLayoutComponent(GroupNode groupNode, NodeObject groupNodeObject)
        {
            var layoutMode = groupNode.layoutMode;
            if (layoutMode == LayoutMode.Horizontal)
            {
                groupNodeObject.gameObject.AddComponent<HorizontalLayoutGroup>();
                HorizontalLayoutGroup layoutGroup = groupNodeObject.gameObject.GetComponent<HorizontalLayoutGroup>();
                layoutGroup.childControlWidth = false;
                layoutGroup.childControlHeight = false;
                layoutGroup.childScaleWidth = false;
                layoutGroup.childScaleHeight = false;
                layoutGroup.childForceExpandWidth = false;
                layoutGroup.childForceExpandHeight = false;
                if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Min)
                {
                    if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperLeft;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerLeft;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleLeft;
                    }
                }
                else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Max)
                {
                    if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperRight;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerRight;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleRight;
                    }
                }
                else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Center)
                {
                    if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperCenter;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerCenter;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    }
                }

                layoutGroup.spacing = groupNode.itemSpacing;
            }
            else
            {
                groupNodeObject.gameObject.AddComponent<VerticalLayoutGroup>();
                VerticalLayoutGroup layoutGroup = groupNodeObject.gameObject.GetComponent<VerticalLayoutGroup>();
                layoutGroup.childControlWidth = false;
                layoutGroup.childControlHeight = false;
                layoutGroup.childScaleWidth = false;
                layoutGroup.childScaleHeight = false;
                layoutGroup.childForceExpandWidth = false;
                layoutGroup.childForceExpandHeight = false;
                if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Min)
                {
                    if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperLeft;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperRight;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.UpperCenter;
                    }
                }
                else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Max)
                {
                    if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerLeft;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerRight;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.LowerCenter;
                    }
                }
                else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Center)
                {
                    if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleLeft;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleRight;
                    }
                    else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
                    {
                        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    }
                }

                layoutGroup.spacing = groupNode.itemSpacing;
            }
        }
    }
}