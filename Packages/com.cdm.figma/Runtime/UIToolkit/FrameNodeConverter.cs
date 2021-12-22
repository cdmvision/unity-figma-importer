using UnityEngine;
using UnityEngine.UIElements;

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
    public class FrameNodeConverter : NodeConverter<FrameNode>
    {
        public override NodeElement Convert(Node node, NodeConvertArgs args)
        {
            var frameNode = (FrameNode) node;
            var nodeElement = GroupNodeConverter.Convert(frameNode, args);
            BuildStyle(frameNode, nodeElement.inlineStyle);
            var children = frameNode.children;
            for (int child = 0; child < children.Length; child++)
            {
                if (args.importer.TryConvertNode(children[child], args, out var childElement))
                {
                    if (frameNode.layoutMode != LayoutMode.None)
                    {
                        HandleFillContainer(frameNode.layoutMode, childElement.node, childElement.inlineStyle, (INodeTransform)childElement.node, (INodeLayout)childElement.node);
                        if (child != children.Length - 1)
                        {
                            if (frameNode.layoutMode == LayoutMode.Horizontal)
                            {
                                childElement.inlineStyle.marginRight =
                                    new StyleLength(new Length(frameNode.itemSpacing, LengthUnit.Pixel));
                            }
                            else if (frameNode.layoutMode == LayoutMode.Vertical)
                            {
                                childElement.inlineStyle.marginBottom =
                                    new StyleLength(new Length(frameNode.itemSpacing, LengthUnit.Pixel));
                            }
                        }
                    }
                    else
                    {
                        HandleConstraints(frameNode.size, childElement.node, childElement.inlineStyle, (INodeTransform)childElement.node, (INodeLayout)childElement.node);
                    }
                    nodeElement.AddChild(childElement);
                }
            }
            return nodeElement;
        }

        private void BuildStyle(FrameNode node, Style style)
        {
            style.position = new StyleEnum<Position>(Position.Relative);
            style.width = new StyleLength(new Length(node.size.x, LengthUnit.Pixel));
            style.height = new StyleLength(new Length(node.size.y, LengthUnit.Pixel));

            if (node.layoutMode != LayoutMode.None)
            {
                HandleAxisSizing(node, style);
            }

            SetClipContent(node, style);
            SetOpacity(node, style);
            SetRotation(node, style);
            var layoutMode = node.layoutMode;
            if (layoutMode != LayoutMode.None)
            {
                HandleAutoLayout(node, style);
            }
            AddPadding(node, style);
            AddBackgroundColor(node, style);
            AddCorners(node, style);
            SetTransformOrigin(style);
        }

        private void SetOpacity(Node node, Style style)
        {
            FrameNode frameNode = (FrameNode) node;
            var opacity = frameNode.opacity;
            style.opacity = new StyleFloat(opacity);
        }

        private void SetClipContent(Node node, Style style)
        {
            FrameNode frameNode = (FrameNode) node;
            if (frameNode.clipsContent)
            {
                style.overflow = new StyleEnum<Overflow>(Overflow.Hidden);
            }
            else
            {
                style.overflow = new StyleEnum<Overflow>(Overflow.Visible);
            }
        }

        private void HandleConstraints(Vector parentSize, Node node, Style style, INodeTransform nodeTransform, INodeLayout nodeLayout)
        {
            //INodeTransform nodeTransform = (INodeTransform) node;
            //INodeLayout nodeLayout = (INodeLayout) node;
            //FrameNode frameNode = (FrameNode) node;
            var relativeTransform = nodeTransform.relativeTransform;
            var position = relativeTransform.GetPosition();
            var constraintX = nodeLayout.constraints.horizontal;
            var constraintY = nodeLayout.constraints.vertical;
            
            if (constraintX == Horizontal.Center)
            {
                style.width = new StyleLength(new Length(nodeTransform.size.x, LengthUnit.Pixel));
                style.left = new StyleLength(new Length(50, LengthUnit.Percent));
                var translateX = parentSize.x/2f - position.x;
                var translateY = parentSize.y/2f - position.y;
                if (constraintY == Vertical.Center)
                {
                    style.translate = 
                        new StyleTranslate(new Translate(new Length(-1*translateX,LengthUnit.Pixel), new Length(-1*translateY,LengthUnit.Pixel), 0));
                }
                else
                {
                    style.translate 
                        = new StyleTranslate(new Translate(new Length(-1*translateX,LengthUnit.Pixel), 0, 0));
                }
            }
            else if (constraintX == Horizontal.Left)
            {
                style.width = new StyleLength(new Length(nodeTransform.size.x, LengthUnit.Pixel));
                style.left = new StyleLength(new Length(position.x, LengthUnit.Pixel));
            }
            else if (constraintX == Horizontal.Right)
            {
                style.width = new StyleLength(new Length(nodeTransform.size.x, LengthUnit.Pixel));
                style.right = new StyleLength(new Length(position.x, LengthUnit.Pixel));
            }
            else if (constraintX == Horizontal.LeftRight)
            {
                var parentWidth = parentSize.x;
                var nodeLeft = position.x;
                var nodeRight = parentWidth - (nodeLeft+nodeTransform.size.x);
                style.left = new StyleLength(new Length(nodeLeft, LengthUnit.Pixel));
                style.right = new StyleLength(new Length(nodeRight, LengthUnit.Pixel));
                style.width = new StyleLength(StyleKeyword.Auto);
            }
            else if (constraintX == Horizontal.Scale)
            {
                var parentWidth = parentSize.x;
                var nodeLeft = position.x;
                var nodeRight = parentWidth - (nodeLeft+nodeTransform.size.x);
                var leftPercentage = (nodeLeft * 100.0f) / parentWidth;
                var rightPercentage = (nodeRight * 100.0f) / parentWidth;
                style.left = new StyleLength(new Length(leftPercentage, LengthUnit.Percent));
                style.right = new StyleLength(new Length(rightPercentage, LengthUnit.Percent));
            }
            
            if (constraintY == Vertical.Center)
            {
                style.height = new StyleLength(new Length(nodeTransform.size.y, LengthUnit.Pixel));
                style.top = new StyleLength(new Length(50, LengthUnit.Percent));
                var translateX = parentSize.x/2f - position.x;
                var translateY = parentSize.y/2f - position.y;
                if (constraintX == Horizontal.Center)
                {
                    style.translate = 
                        new StyleTranslate(new Translate(new Length(-1*translateX,LengthUnit.Pixel), new Length(-1*translateY,LengthUnit.Pixel), 0));
                }
                else
                {
                    style.translate 
                        = new StyleTranslate(new Translate(0, new Length(-1*translateY,LengthUnit.Pixel), 0));
                }
            }
            else if (constraintY == Vertical.Top)
            {
                style.height = new StyleLength(new Length(nodeTransform.size.y, LengthUnit.Pixel));
                style.top = new StyleLength(new Length(position.y, LengthUnit.Pixel));
            }
            else if (constraintY == Vertical.Bottom)
            {
                style.height = new StyleLength(new Length(nodeTransform.size.y, LengthUnit.Pixel));
                style.bottom = new StyleLength(new Length(position.y, LengthUnit.Pixel));
            }
            else if (constraintY == Vertical.TopBottom)
            {
                var parentHeight = parentSize.y;
                var nodeTop = position.y;
                var nodeBottom = parentHeight - (nodeTop+nodeTransform.size.y);
                style.top = new StyleLength(new Length(nodeTop, LengthUnit.Pixel));
                style.bottom = new StyleLength(new Length(nodeBottom, LengthUnit.Pixel));
                style.height = new StyleLength(StyleKeyword.Auto);
            }
            else if (constraintY == Vertical.Scale)
            {
                var parentHeight = parentSize.y;
                var nodeTop = position.y;
                var nodeBottom = parentHeight - (nodeTop+nodeTransform.size.y);
                var topPercentage = (nodeTop * 100.0f) / parentHeight;
                var bottomPercentage = (nodeBottom * 100.0f) / parentHeight;
                style.top = new StyleLength(new Length(topPercentage, LengthUnit.Percent));
                style.bottom = new StyleLength(new Length(bottomPercentage, LengthUnit.Percent));
            }
        }

        private void HandleFillContainer(LayoutMode layoutMode, Node node, Style style, INodeTransform nodeTransform, INodeLayout nodeLayout)
        {
            //INodeLayout nodeLayout = (INodeLayout) node;
            //INodeTransform nodeTransform = (INodeTransform) node;
            //FrameNode frameNode = (FrameNode) node;
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
            
        }

        private void HandleAxisSizing(Node node, Style style)
        {
            FrameNode frameNode = (FrameNode) node;
            LayoutMode mode = frameNode.layoutMode;
            if (frameNode.primaryAxisSizingMode == AxisSizingMode.Auto)
            {
                if (mode == LayoutMode.Horizontal)
                {
                    Debug.Log(frameNode.name + ": Primary axis auto sizing mode is not supported, setting height to fixed.");
                    style.width = new StyleLength(new Length(frameNode.size.x, LengthUnit.Pixel));
                }
                else if (mode == LayoutMode.Vertical)
                {
                    Debug.Log(frameNode.name + ": Primary axis auto sizing mode is not supported, setting width to fixed.");
                    style.height = new StyleLength(new Length(frameNode.size.y, LengthUnit.Pixel));
                }
            }
            else if(frameNode.primaryAxisSizingMode == AxisSizingMode.Fixed)
            {
                if (mode == LayoutMode.Horizontal)
                {
                    style.width = new StyleLength(new Length(frameNode.size.x, LengthUnit.Pixel));
                }
                else if (mode == LayoutMode.Vertical)
                {
                    style.height = new StyleLength(new Length(frameNode.size.y, LengthUnit.Pixel));
                }
            }

            if (frameNode.counterAxisSizingMode == AxisSizingMode.Auto)
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
            else if(frameNode.counterAxisSizingMode == AxisSizingMode.Fixed)
            {
                if (mode == LayoutMode.Horizontal)
                {
                    style.height = new StyleLength(new Length(frameNode.size.y, LengthUnit.Pixel));
                }
                else if (mode == LayoutMode.Vertical)
                {
                    style.width = new StyleLength(new Length(frameNode.size.x, LengthUnit.Pixel));
                }
            }
        }
        
        private void SetRotation(Node node, Style style)
        {
            FrameNode frameNode = (FrameNode) node;
            var relativeTransform = frameNode.relativeTransform;
            var rotation = relativeTransform.GetRotationAngle();
            if (rotation != 0.0f)
            {
                style.rotate = new StyleRotate(new Rotate(rotation));
            }
        }

        private void HandleAutoLayout(Node node, Style style)
        {
            FrameNode frameNode = (FrameNode) node;
            var layoutMode = frameNode.layoutMode;
            style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            if (layoutMode == LayoutMode.Horizontal)
            {
                style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            }
            else
            {
                style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
            }
                
            if (frameNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Min)
            {
                style.justifyContent = new StyleEnum<Justify>(Justify.FlexStart);
            }
            else if (frameNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Max)
            {
                style.justifyContent = new StyleEnum<Justify>?(Justify.FlexEnd);
            }
            else if(frameNode.primaryAxisAlignItems == PrimaryAxisAlignItems.Center)
            {
                style.justifyContent = new StyleEnum<Justify>?(Justify.Center);
            }
            else if(frameNode.primaryAxisAlignItems == PrimaryAxisAlignItems.SpaceBetween)
            {
                style.justifyContent = new StyleEnum<Justify>?(Justify.SpaceBetween);
            }
                
            if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Min)
            {
                style.alignItems = new StyleEnum<Align>(Align.FlexStart);
            }
            else if (frameNode.counterAxisAlignItems == CounterAxisAlignItems.Max)
            {
                style.alignItems = new StyleEnum<Align>(Align.FlexEnd);
            }
            else if(frameNode.counterAxisAlignItems == CounterAxisAlignItems.Center)
            {
                style.alignItems = new StyleEnum<Align>(Align.Center);
            }
        }

        private void AddPadding(Node node, Style style)
        {
            FrameNode frameNode = (FrameNode) node;
            style.paddingTop = new StyleLength(new Length(frameNode.paddingTop, LengthUnit.Pixel));
            style.paddingLeft = new StyleLength(new Length(frameNode.paddingLeft, LengthUnit.Pixel));
            style.paddingBottom = new StyleLength(new Length(frameNode.paddingBottom, LengthUnit.Pixel));
            style.paddingRight = new StyleLength(new Length(frameNode.paddingRight, LengthUnit.Pixel));
        }
        
        private void SetTransformOrigin(Style style)
        {
            // Figma transform pivot is located on the top left.
            style.transformOrigin = new StyleTransformOrigin(new TransformOrigin(Length.Percent(0f), Length.Percent(0f), 0.0f));
        }

        private void AddCorners(Node node, Style style)
        {
            FrameNode frameNode = (FrameNode) node;
            var cornerRadius = frameNode.cornerRadius;
            var cornerRadii = frameNode.rectangleCornerRadii;
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
                    style.borderTopLeftRadius = new StyleLength(new Length((float)cornerRadius, LengthUnit.Pixel));
                    style.borderTopRightRadius = new StyleLength(new Length((float)cornerRadius, LengthUnit.Pixel));
                    style.borderBottomRightRadius = new StyleLength(new Length((float)cornerRadius, LengthUnit.Pixel));
                    style.borderBottomLeftRadius = new StyleLength(new Length((float)cornerRadius, LengthUnit.Pixel));
                }
            }
            
            var strokes = frameNode.strokes;
            if (strokes.Count > 0)
            {
                var strokeWeight = frameNode.strokeWeight;
                if (strokeWeight.HasValue)
                {
                    style.borderTopWidth = new StyleFloat((float)strokeWeight);
                    style.borderLeftWidth = new StyleFloat((float)strokeWeight);
                    style.borderBottomWidth = new StyleFloat((float)strokeWeight);
                    style.borderRightWidth = new StyleFloat((float)strokeWeight);
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

        private void AddBackgroundColor(Node node, Style style)
        {
            FrameNode frameNode = (FrameNode) node;
            var fills = frameNode.fills;
            if (fills.Count > 0)
            {
                var solidColor = (SolidPaint) fills[0];
                style.backgroundColor = new StyleColor(solidColor.color);
            }
        }
    }
}