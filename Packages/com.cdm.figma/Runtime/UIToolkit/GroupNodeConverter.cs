using UnityEngine.UIElements;
using UnityEngine;

namespace Cdm.Figma.UIToolkit
{
    /// <summary>
    /// PROPERTIES THAT ARE NOT SUPPORTED IN UI TOOLKIT:
    /// preserveRatio,
    /// strokeAlign,
    /// effects,
    /// isMaskBoolean,
    /// isMaskOutline,
    /// blendMode
    /// </summary>
    public class GroupNodeConverter : NodeConverter<GroupNode>
    {
        public static NodeElement Convert(NodeElement parentElement, GroupNode groupNode, NodeConvertArgs args)
        {
            var element = NodeElement.New<VisualElement>(groupNode, args);
            if (groupNode is FrameNode)
            {
                BuildStyle(groupNode, element.inlineStyle);
                BuildChildren(groupNode, element, args);
            }
            else
            {
                BuildChildren(groupNode, parentElement, args);
            }

            return element;
        }

        public override NodeElement Convert(NodeElement parentElement, Node node, NodeConvertArgs args)
        {
            return Convert(parentElement, (GroupNode) node, args);
        }

        private static void BuildChildren(GroupNode currentNode, NodeElement parentElement, NodeConvertArgs args)
        {
            GroupNode parent = (GroupNode) parentElement.node;
            var children = currentNode.children;
            if (children != null)
            {
                for (int child = 0; child < children.Length; child++)
                {
                    if (args.importer.TryConvertNode(parentElement, children[child], args, out var childElement))
                    {
                        var importChild = childElement.node is FrameNode or VectorNode;
                        //do not import child group nodes
                        if (importChild)
                        {
                            if (currentNode.layoutMode != LayoutMode.None)
                            {
                                HandleFillContainer(parent.layoutMode, childElement.inlineStyle,
                                    (INodeTransform) childElement.node, (INodeLayout) childElement.node);
                                if (child != children.Length - 1)
                                {
                                    if (currentNode.layoutMode == LayoutMode.Horizontal)
                                    {
                                        childElement.inlineStyle.marginRight =
                                            new StyleLength(new Length(currentNode.itemSpacing, LengthUnit.Pixel));
                                    }
                                    else if (currentNode.layoutMode == LayoutMode.Vertical)
                                    {
                                        childElement.inlineStyle.marginBottom =
                                            new StyleLength(new Length(currentNode.itemSpacing, LengthUnit.Pixel));
                                    }
                                }
                            }
                            else
                            {
                                if (currentNode.type == NodeType.Group)
                                {
                                    HandleConstraints(parent.size, currentNode.relativeTransform.GetPosition(), true,
                                        childElement.inlineStyle,
                                        (INodeTransform) childElement.node, (INodeLayout) childElement.node);
                                }
                                else
                                {
                                    HandleConstraints(parent.size, currentNode.relativeTransform.GetPosition(), false,
                                        childElement.inlineStyle,
                                        (INodeTransform) childElement.node, (INodeLayout) childElement.node);
                                }
                            }

                            if (childElement != parentElement)
                            {
                                parentElement.AddChild(childElement);
                            }
                        }
                    }
                }
            }
        }

        private static void BuildStyle(GroupNode node, Style style)
        {
            //unity ui toolkit automatically sets shrink to 1, we don't want that.
            style.flexShrink = new StyleFloat(0f);
            style.position = new StyleEnum<Position>(Position.Relative);
            style.width = new StyleLength(new Length(node.size.x, LengthUnit.Pixel));
            style.height = new StyleLength(new Length(node.size.y, LengthUnit.Pixel));

            if (node.layoutMode != LayoutMode.None)
            {
                HandleAxisSizing(node, style);
                HandleAutoLayout(node, style);
            }

            SetClipContent(node, style);
            SetOpacity(node, style);
            SetRotation(node, style);
            AddPadding(node, style);
            AddBackgroundColor(node, style);
            AddCorners(node, style);
            SetTransformOrigin(style);
        }

        private static void SetOpacity(Node node, Style style)
        {
            GroupNode groupNode = (GroupNode) node;
            var opacity = groupNode.opacity;
            style.opacity = new StyleFloat(opacity);
        }

        private static void SetClipContent(Node node, Style style)
        {
            GroupNode groupNode = (GroupNode) node;
            if (groupNode.clipsContent)
            {
                style.overflow = new StyleEnum<Overflow>(Overflow.Hidden);
            }
            else
            {
                style.overflow = new StyleEnum<Overflow>(Overflow.Visible);
            }
        }

        private static void HandleConstraints(Vector parentSize, Vector2 parentPos, bool isParentGroup, Style style,
            INodeTransform nodeTransform,
            INodeLayout nodeLayout)
        {
            style.position = new StyleEnum<Position>(Position.Absolute);
            var relativeTransform = nodeTransform.relativeTransform;
            var position = relativeTransform.GetPosition();
            var positionX = position.x;
            var positionY = position.y;
            if (isParentGroup)
            {
                positionX = position.x + parentPos.x;
                positionY = position.y + parentPos.y;
            }

            var constraintX = nodeLayout.constraints.horizontal;
            var constraintY = nodeLayout.constraints.vertical;

            if (constraintX == Horizontal.Center)
            {
                style.width = new StyleLength(new Length(nodeTransform.size.x, LengthUnit.Pixel));
                style.left = new StyleLength(new Length(50, LengthUnit.Percent));
                var translateX = parentSize.x / 2f - (positionX);
                var translateY = parentSize.y / 2f - (positionY);

                if (constraintY == Vertical.Center)
                {
                    style.translate =
                        new StyleTranslate(new Translate(new Length(-1 * translateX, LengthUnit.Pixel),
                            new Length(-1 * translateY, LengthUnit.Pixel), 0));
                }
                else
                {
                    style.translate
                        = new StyleTranslate(new Translate(new Length(-1 * translateX, LengthUnit.Pixel), 0, 0));
                }
            }
            else if (constraintX == Horizontal.Left)
            {
                style.width = new StyleLength(new Length(nodeTransform.size.x, LengthUnit.Pixel));
                style.left = new StyleLength(new Length(positionX, LengthUnit.Pixel));
            }
            else if (constraintX == Horizontal.Right)
            {
                style.width = new StyleLength(new Length(nodeTransform.size.x, LengthUnit.Pixel));
                var right = parentSize.x - (positionX + nodeTransform.size.x);
                style.right = new StyleLength(new Length(right, LengthUnit.Pixel));
            }
            else if (constraintX == Horizontal.LeftRight)
            {
                var parentWidth = parentSize.x;
                var nodeLeft = positionX;
                var nodeRight = parentWidth - (nodeLeft + nodeTransform.size.x);
                style.left = new StyleLength(new Length(nodeLeft, LengthUnit.Pixel));
                style.right = new StyleLength(new Length(nodeRight, LengthUnit.Pixel));
                style.width = new StyleLength(StyleKeyword.Auto);
            }
            else if (constraintX == Horizontal.Scale)
            {
                var parentWidth = parentSize.x;
                var nodeLeft = positionX;
                var nodeRight = parentWidth - (nodeLeft + nodeTransform.size.x);
                var leftPercentage = (nodeLeft * 100) / parentWidth;
                leftPercentage = float.Parse(leftPercentage.ToString("F2"));
                var rightPercentage = (nodeRight * 100) / parentWidth;
                rightPercentage = float.Parse(rightPercentage.ToString("F2"));
                style.left = new StyleLength(new Length(leftPercentage, LengthUnit.Percent));
                style.right = new StyleLength(new Length(rightPercentage, LengthUnit.Percent));
                style.width = new StyleLength(StyleKeyword.Auto);
            }

            if (constraintY == Vertical.Center)
            {
                style.height = new StyleLength(new Length(nodeTransform.size.y, LengthUnit.Pixel));
                style.top = new StyleLength(new Length(50, LengthUnit.Percent));
                var translateX = parentSize.x / 2f - (positionX);
                var translateY = parentSize.y / 2f - (positionY);

                if (constraintX == Horizontal.Center)
                {
                    style.translate =
                        new StyleTranslate(new Translate(new Length(-1 * translateX, LengthUnit.Pixel),
                            new Length(-1 * translateY, LengthUnit.Pixel), 0));
                }
                else
                {
                    style.translate
                        = new StyleTranslate(new Translate(0, new Length(-1 * translateY, LengthUnit.Pixel), 0));
                }
            }
            else if (constraintY == Vertical.Top)
            {
                style.height = new StyleLength(new Length(nodeTransform.size.y, LengthUnit.Pixel));
                style.top = new StyleLength(new Length(positionY, LengthUnit.Pixel));
            }
            else if (constraintY == Vertical.Bottom)
            {
                style.height = new StyleLength(new Length(nodeTransform.size.y, LengthUnit.Pixel));
                var bottom = parentSize.y - (positionY + nodeTransform.size.y);
                style.bottom = new StyleLength(new Length(bottom, LengthUnit.Pixel));
            }
            else if (constraintY == Vertical.TopBottom)
            {
                var parentHeight = parentSize.y;
                var nodeTop = positionY;
                var nodeBottom = parentHeight - (nodeTop + nodeTransform.size.y);
                style.top = new StyleLength(new Length(nodeTop, LengthUnit.Pixel));
                style.bottom = new StyleLength(new Length(nodeBottom, LengthUnit.Pixel));
                style.height = new StyleLength(StyleKeyword.Auto);
            }
            else if (constraintY == Vertical.Scale)
            {
                var parentHeight = parentSize.y;
                var nodeTop = positionY;
                var nodeBottom = parentHeight - (nodeTop + nodeTransform.size.y);
                var topPercentage = (nodeTop * 100) / parentHeight;
                topPercentage = float.Parse(topPercentage.ToString("F2"));
                var bottomPercentage = (nodeBottom * 100) / parentHeight;
                bottomPercentage = float.Parse(bottomPercentage.ToString("F2"));
                style.top = new StyleLength(new Length(topPercentage, LengthUnit.Percent));
                style.bottom = new StyleLength(new Length(bottomPercentage, LengthUnit.Percent));
                style.height = new StyleLength(StyleKeyword.Auto);
            }
        }

        private static void HandleFillContainer(LayoutMode layoutMode, Style style, INodeTransform nodeTransform,
            INodeLayout nodeLayout)
        {
            style.position = new StyleEnum<Position>(Position.Relative);
            if (nodeLayout.layoutAlign == LayoutAlign.Stretch)
            {
                style.alignSelf = new StyleEnum<Align>(Align.Stretch);
                if (layoutMode == LayoutMode.Horizontal)
                {
                    style.height = new StyleLength(StyleKeyword.Auto);
                    style.width = new StyleLength(new Length(nodeTransform.size.x, LengthUnit.Pixel));
                }
                else
                {
                    style.width = new StyleLength(StyleKeyword.Auto);
                    style.height = new StyleLength(new Length(nodeTransform.size.y, LengthUnit.Pixel));
                }
            }
            else
            {
                style.width = new StyleLength(new Length(nodeTransform.size.x, LengthUnit.Pixel));
                style.height = new StyleLength(new Length(nodeTransform.size.y, LengthUnit.Pixel));
            }

            if (nodeLayout.layoutGrow.HasValue)
            {
                style.flexGrow = new StyleFloat(nodeLayout.layoutGrow.Value);
            }
        }

        private static void HandleAxisSizing(Node node, Style style)
        {
            GroupNode groupNode = (GroupNode) node;
            LayoutMode mode = groupNode.layoutMode;
            if (groupNode.primaryAxisSizingMode == AxisSizingMode.Auto)
            {
                if (mode == LayoutMode.Horizontal)
                {
                    Debug.Log(groupNode.name +
                              ": Primary axis auto sizing mode is not supported, setting height to fixed.");
                    style.width = new StyleLength(new Length(groupNode.size.x, LengthUnit.Pixel));
                }
                else if (mode == LayoutMode.Vertical)
                {
                    Debug.Log(groupNode.name +
                              ": Primary axis auto sizing mode is not supported, setting width to fixed.");
                    style.height = new StyleLength(new Length(groupNode.size.y, LengthUnit.Pixel));
                }
            }
            else if (groupNode.primaryAxisSizingMode == AxisSizingMode.Fixed)
            {
                if (mode == LayoutMode.Horizontal)
                {
                    style.width = new StyleLength(new Length(groupNode.size.x, LengthUnit.Pixel));
                }
                else if (mode == LayoutMode.Vertical)
                {
                    style.height = new StyleLength(new Length(groupNode.size.y, LengthUnit.Pixel));
                }
            }

            if (groupNode.counterAxisSizingMode == AxisSizingMode.Auto)
            {
                if (mode == LayoutMode.Horizontal)
                {
                    style.height = new StyleLength(StyleKeyword.Auto);
                }
                else if (mode == LayoutMode.Vertical)
                {
                    style.width = new StyleLength(StyleKeyword.Auto);
                }
            }
            else if (groupNode.counterAxisSizingMode == AxisSizingMode.Fixed)
            {
                if (mode == LayoutMode.Horizontal)
                {
                    style.height = new StyleLength(new Length(groupNode.size.y, LengthUnit.Pixel));
                }
                else if (mode == LayoutMode.Vertical)
                {
                    style.width = new StyleLength(new Length(groupNode.size.x, LengthUnit.Pixel));
                }
            }
        }

        private static void SetRotation(Node node, Style style)
        {
            GroupNode groupNode = (GroupNode) node;
            var relativeTransform = groupNode.relativeTransform;
            var rotation = relativeTransform.GetRotationAngle();
            if (rotation != 0.0f)
            {
                style.rotate = new StyleRotate(new Rotate(rotation));
            }
        }

        private static void HandleAutoLayout(Node node, Style style)
        {
            GroupNode groupNode = (GroupNode) node;
            var layoutMode = groupNode.layoutMode;
            style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            if (layoutMode == LayoutMode.Horizontal)
            {
                style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            }
            else
            {
                style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
            }

            if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Min)
            {
                style.justifyContent = new StyleEnum<Justify>(Justify.FlexStart);
            }
            else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Max)
            {
                style.justifyContent = new StyleEnum<Justify>?(Justify.FlexEnd);
            }
            else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Center)
            {
                style.justifyContent = new StyleEnum<Justify>?(Justify.Center);
            }
            else if (groupNode.primaryAxisAlignItems == PrimaryAxisAlignItems.SpaceBetween)
            {
                style.justifyContent = new StyleEnum<Justify>?(Justify.SpaceBetween);
            }

            if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
            {
                style.alignItems = new StyleEnum<Align>(Align.FlexStart);
            }
            else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
            {
                style.alignItems = new StyleEnum<Align>(Align.FlexEnd);
            }
            else if (groupNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
            {
                style.alignItems = new StyleEnum<Align>(Align.Center);
            }
        }

        private static void AddPadding(Node node, Style style)
        {
            GroupNode groupNode = (GroupNode) node;
            style.paddingTop = new StyleLength(new Length(groupNode.paddingTop, LengthUnit.Pixel));
            style.paddingLeft = new StyleLength(new Length(groupNode.paddingLeft, LengthUnit.Pixel));
            style.paddingBottom = new StyleLength(new Length(groupNode.paddingBottom, LengthUnit.Pixel));
            style.paddingRight = new StyleLength(new Length(groupNode.paddingRight, LengthUnit.Pixel));
        }

        private static void SetTransformOrigin(Style style)
        {
            // Figma transform pivot is located on the top left.
            style.transformOrigin =
                new StyleTransformOrigin(new TransformOrigin(Length.Percent(0f), Length.Percent(0f), 0.0f));
        }

        private static void AddCorners(Node node, Style style)
        {
            GroupNode groupNode = (GroupNode) node;
            var cornerRadius = groupNode.cornerRadius;
            var cornerRadii = groupNode.rectangleCornerRadii;
            //If element has 4 different radius for each corner, use this
            if (cornerRadii != null)
            {
                style.borderTopLeftRadius = new StyleLength(new Length(cornerRadii[0], LengthUnit.Pixel));
                style.borderTopRightRadius = new StyleLength(new Length(cornerRadii[1], LengthUnit.Pixel));
                style.borderBottomRightRadius = new StyleLength(new Length(cornerRadii[2], LengthUnit.Pixel));
                style.borderBottomLeftRadius = new StyleLength(new Length(cornerRadii[3], LengthUnit.Pixel));
            }
            //Element does not have 4 different radius for each corner...
            else
            {
                //Does it have cornerRadius?
                if (cornerRadius.HasValue)
                {
                    style.borderTopLeftRadius = new StyleLength(new Length((float) cornerRadius, LengthUnit.Pixel));
                    style.borderTopRightRadius = new StyleLength(new Length((float) cornerRadius, LengthUnit.Pixel));
                    style.borderBottomRightRadius = new StyleLength(new Length((float) cornerRadius, LengthUnit.Pixel));
                    style.borderBottomLeftRadius = new StyleLength(new Length((float) cornerRadius, LengthUnit.Pixel));
                }
            }

            var strokes = groupNode.strokes;
            if (strokes.Count > 0)
            {
                var strokeWeight = groupNode.strokeWeight;
                if (strokeWeight.HasValue)
                {
                    style.borderTopWidth = new StyleFloat((float) strokeWeight);
                    style.borderLeftWidth = new StyleFloat((float) strokeWeight);
                    style.borderBottomWidth = new StyleFloat((float) strokeWeight);
                    style.borderRightWidth = new StyleFloat((float) strokeWeight);
                }

                //only getting the base color
                var solidColor = (SolidPaint) strokes[0];
                var strokeColorBlended = solidColor.color;
                style.borderTopColor = new StyleColor(strokeColorBlended);
                style.borderLeftColor = new StyleColor(strokeColorBlended);
                style.borderBottomColor = new StyleColor(strokeColorBlended);
                style.borderRightColor = new StyleColor(strokeColorBlended);
            }
        }

        private static void AddBackgroundColor(Node node, Style style)
        {
            GroupNode groupNode = (GroupNode) node;
            var fills = groupNode.fills;
            if (fills.Count > 0)
            {
                var solidColor = (SolidPaint) fills[0];
                style.backgroundColor = new StyleColor(solidColor.color);
            }
        }
    }
}