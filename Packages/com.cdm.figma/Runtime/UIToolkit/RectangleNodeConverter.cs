using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class RectangleNodeConverter : NodeConverter<RectangleNode>
    {
        public override NodeElement Convert(Node node, NodeConvertArgs args)
        {
            var rectangleNode = (RectangleNode) node;
            var nodeElement = NodeElement.New<VisualElement>(rectangleNode, args);
            BuildStyle(rectangleNode, nodeElement.inlineStyle);
            return nodeElement;
        }

        private void BuildStyle(RectangleNode node, Style style)
        {
            GroupNode parent = new GroupNode();
            
            if (node.hasParent)
            {
                if (node.parent.type == NodeType.Frame || node.parent.type == NodeType.Group)
                {
                    if (node.parent.type == NodeType.Frame)
                        parent = (FrameNode) node.parent;
                    else
                        parent = (GroupNode) node.parent;
                }
            }

            if (NodeHasVisualParent(node))
            {
                if (parent.layoutMode != LayoutMode.None)
                {
                    SetPositionRelative(parent.layoutMode, node, style);
                }

                else
                {
                    SetPositionAbsolute(parent.size, node, style);
                }
            }
            else if (!NodeHasVisualParent(node))
            {
                SetPositionAbsolute(parent.size, node, style);
            }
            
            SetOpacity(node, style);
            SetRotation(node, style);
            AddBackgroundColor(node, style);
            AddCorners(node, style);
            SetTransformOrigin(style);
        }
        
        private void SetOpacity(Node node, Style style)
        {
            RectangleNode rectangleNode = (RectangleNode) node;
            var opacity = rectangleNode.opacity;
            style.opacity = new StyleFloat(opacity);
        }
        
        private bool NodeHasVisualParent(Node node)
        {
            RectangleNode rectangleNode = (RectangleNode) node;
            if (rectangleNode.parent.type == NodeType.Frame || rectangleNode.parent.type == NodeType.Group || 
                rectangleNode.parent.type == NodeType.Component || rectangleNode.parent.type == NodeType.ComponentSet ||
                rectangleNode.parent.type == NodeType.Instance)
            {
                return true;
            }

            return false;
        }

        private void SetPositionAbsolute(Vector parentSize, Node node, Style style)
        {
            RectangleNode rectangleNode = (RectangleNode) node;
            style.position = new StyleEnum<Position>(Position.Absolute);
            var relativeTransform = rectangleNode.relativeTransform;
            var position = relativeTransform.GetPosition();
            var constraintX = rectangleNode.constraints.horizontal;
            var constraintY = rectangleNode.constraints.vertical;
            
            if (constraintX == Horizontal.Center)
            {
                style.width = new StyleLength(new Length(rectangleNode.size.x, LengthUnit.Pixel));
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
                style.width = new StyleLength(new Length(rectangleNode.size.x, LengthUnit.Pixel));
                style.left = new StyleLength(new Length(position.x, LengthUnit.Pixel));
            }
            else if (constraintX == Horizontal.Right)
            {
                style.width = new StyleLength(new Length(rectangleNode.size.x, LengthUnit.Pixel));
                style.right = new StyleLength(new Length(position.x, LengthUnit.Pixel));
            }
            else if (constraintX == Horizontal.LeftRight)
            {
                var parentWidth = parentSize.x;
                var nodeLeft = position.x;
                var nodeRight = parentWidth - (nodeLeft+rectangleNode.size.x);
                style.left = new StyleLength(new Length(nodeLeft, LengthUnit.Pixel));
                style.right = new StyleLength(new Length(nodeRight, LengthUnit.Pixel));
                style.width = new StyleLength(StyleKeyword.Auto);
            }
            else if (constraintX == Horizontal.Scale)
            {
                var parentWidth = parentSize.x;
                var nodeLeft = position.x;
                var nodeRight = parentWidth - (nodeLeft+rectangleNode.size.x);
                var leftPercentage = (nodeLeft * 100.0f) / parentWidth;
                var rightPercentage = (nodeRight * 100.0f) / parentWidth;
                style.left = new StyleLength(new Length(leftPercentage, LengthUnit.Percent));
                style.right = new StyleLength(new Length(rightPercentage, LengthUnit.Percent));
            }
            
            if (constraintY == Vertical.Center)
            {
                style.height = new StyleLength(new Length(rectangleNode.size.y, LengthUnit.Pixel));
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
                style.height = new StyleLength(new Length(rectangleNode.size.y, LengthUnit.Pixel));
                style.top = new StyleLength(new Length(position.y, LengthUnit.Pixel));
            }
            else if (constraintY == Vertical.Bottom)
            {
                style.height = new StyleLength(new Length(rectangleNode.size.y, LengthUnit.Pixel));
                style.bottom = new StyleLength(new Length(position.y, LengthUnit.Pixel));
            }
            else if (constraintY == Vertical.TopBottom)
            {
                var parentHeight = parentSize.y;
                var nodeTop = position.y;
                var nodeBottom = parentHeight - (nodeTop+rectangleNode.size.y);
                style.top = new StyleLength(new Length(nodeTop, LengthUnit.Pixel));
                style.bottom = new StyleLength(new Length(nodeBottom, LengthUnit.Pixel));
                style.height = new StyleLength(StyleKeyword.Auto);
            }
            else if (constraintY == Vertical.Scale)
            {
                var parentHeight = parentSize.y;
                var nodeTop = position.y;
                var nodeBottom = parentHeight - (nodeTop+rectangleNode.size.y);
                var topPercentage = (nodeTop * 100.0f) / parentHeight;
                var bottomPercentage = (nodeBottom * 100.0f) / parentHeight;
                style.top = new StyleLength(new Length(topPercentage, LengthUnit.Percent));
                style.bottom = new StyleLength(new Length(bottomPercentage, LengthUnit.Percent));
            }
        }

        private void SetPositionRelative(LayoutMode parentLayoutMode, Node node, Style style)
        {
            RectangleNode rectangleNode = (RectangleNode) node;
            style.position = new StyleEnum<Position>(Position.Relative);
            if (rectangleNode.layoutAlign == LayoutAlign.Stretch)
            {
                style.alignSelf = new StyleEnum<Align>(Align.Stretch);
                if (parentLayoutMode == LayoutMode.Horizontal)
                {
                    style.height = new StyleLength(StyleKeyword.Auto);
                    style.width = new StyleLength(new Length(rectangleNode.size.x, LengthUnit.Pixel));
                }
                else
                {
                    style.width = new StyleLength(StyleKeyword.Auto);
                    style.height = new StyleLength(new Length(rectangleNode.size.y, LengthUnit.Pixel));
                }
            }
            else
            {
                style.width = new StyleLength(new Length(rectangleNode.size.x, LengthUnit.Pixel));
                style.height = new StyleLength(new Length(rectangleNode.size.y, LengthUnit.Pixel));
            }
            style.flexGrow = new StyleFloat(rectangleNode.layoutGrow);
        }

        private void SetRotation(Node node, Style style)
        {
            RectangleNode rectangleNode = (RectangleNode) node;
            var relativeTransform = rectangleNode.relativeTransform;
            var rotation = relativeTransform.GetRotationAngle();
            if (rotation != 0.0f)
            {
                style.rotate = new StyleRotate(new Rotate(rotation));
            }
        }
        
        private void SetTransformOrigin(Style style)
        {
            // Figma transform pivot is located on the top left.
            style.transformOrigin = new StyleTransformOrigin(new TransformOrigin(Length.Percent(0f), Length.Percent(0f), 0.0f));
        }

        private void AddCorners(Node node, Style style)
        {
            RectangleNode rectangleNode = (RectangleNode) node;
            var cornerRadius = rectangleNode.cornerRadius;
            var cornerRadii = rectangleNode.rectangleCornerRadii;
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
            
            var strokes = rectangleNode.strokes;
            if (strokes.Length > 0)
            {
                var strokeWeight = rectangleNode.strokeWeight;
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
            RectangleNode rectangleNode = (RectangleNode) node;
            var fills = rectangleNode.fills;
            if (fills.Length > 0)
            {
                //only getting the base color
                var solidColor = (SolidPaint) fills[0];
                var fillColorBlended = solidColor.color;
                style.backgroundColor = new StyleColor(fillColorBlended);
            }
        }
    }
}