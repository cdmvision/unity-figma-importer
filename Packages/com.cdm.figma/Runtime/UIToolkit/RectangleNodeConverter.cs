using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
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

            /*positioning and size*/
            if (parent.layoutMode != LayoutMode.None)
            {
                style.position = new StyleEnum<Position>(Position.Relative);
                if (node.layoutAlign == LayoutAlign.Stretch)
                {
                    style.alignSelf = new StyleEnum<Align>(Align.Stretch);
                    if (parent.layoutMode == LayoutMode.Horizontal)
                    {
                        style.height = new StyleLength(StyleKeyword.Auto);
                        style.width = new StyleLength(new Length(node.size.x, LengthUnit.Pixel));
                    }
                    else
                    {
                        style.width = new StyleLength(StyleKeyword.Auto);
                        style.height = new StyleLength(new Length(node.size.y, LengthUnit.Pixel));
                    }
                }
                else
                {
                    style.width = new StyleLength(new Length(node.size.x, LengthUnit.Pixel));
                    style.height = new StyleLength(new Length(node.size.y, LengthUnit.Pixel));
                }
                
                var relativeTransform = node.relativeTransform;
                var rotation = relativeTransform.GetRotationAngle();
                if (rotation != 0.0f)
                {
                    style.rotate = new StyleRotate(new Rotate(rotation));
                }
                style.flexGrow = new StyleFloat(node.layoutGrow);
            }

            else
            {
                style.position = new StyleEnum<Position>(Position.Absolute);
                var relativeTransform = node.relativeTransform;
                var position = relativeTransform.GetPosition();
                var rotation = relativeTransform.GetRotationAngle();
                if (rotation != 0.0f)
                {
                    style.rotate = new StyleRotate(new Rotate(rotation));
                }
                var constraintX = node.constraints.horizontal;
                var constraintY = node.constraints.vertical;
                
                if (constraintX == Horizontal.Center)
                {
                    style.width = new StyleLength(new Length(node.size.x, LengthUnit.Pixel));
                    style.left = new StyleLength(new Length(50, LengthUnit.Percent));
                    var translateX = parent.size.x/2f - position.x;
                    var translateY = parent.size.y/2f - position.y;
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
                    style.width = new StyleLength(new Length(node.size.x, LengthUnit.Pixel));
                    style.left = new StyleLength(new Length(position.x, LengthUnit.Pixel));
                }
                else if (constraintX == Horizontal.Right)
                {
                    style.width = new StyleLength(new Length(node.size.x, LengthUnit.Pixel));
                    style.right = new StyleLength(new Length(position.x, LengthUnit.Pixel));
                }
                else if (constraintX == Horizontal.LeftRight)
                {
                    var parentWidth = parent.size.x;
                    var nodeLeft = position.x;
                    var nodeRight = parentWidth - (nodeLeft+node.size.x);
                    style.left = new StyleLength(new Length(nodeLeft, LengthUnit.Pixel));
                    style.right = new StyleLength(new Length(nodeRight, LengthUnit.Pixel));
                    style.width = new StyleLength(StyleKeyword.Auto);
                }
                else if (constraintX == Horizontal.Scale)
                {
                    var parentWidth = parent.size.x;
                    var nodeLeft = position.x;
                    var nodeRight = parentWidth - (nodeLeft+node.size.x);
                    var leftPercentage = (nodeLeft * 100.0f) / parentWidth;
                    var rightPercentage = (nodeRight * 100.0f) / parentWidth;
                    style.left = new StyleLength(new Length(leftPercentage, LengthUnit.Percent));
                    style.right = new StyleLength(new Length(rightPercentage, LengthUnit.Percent));
                }
                
                if (constraintY == Vertical.Center)
                {
                    style.height = new StyleLength(new Length(node.size.y, LengthUnit.Pixel));
                    style.top = new StyleLength(new Length(50, LengthUnit.Percent));
                    var translateX = parent.size.x/2f - position.x;
                    var translateY = parent.size.y/2f - position.y;
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
                    style.height = new StyleLength(new Length(node.size.y, LengthUnit.Pixel));
                    style.top = new StyleLength(new Length(position.y, LengthUnit.Pixel));
                }
                else if (constraintY == Vertical.Bottom)
                {
                    style.height = new StyleLength(new Length(node.size.y, LengthUnit.Pixel));
                    style.bottom = new StyleLength(new Length(position.y, LengthUnit.Pixel));
                }
                else if (constraintY == Vertical.TopBottom)
                {
                    var parentHeight = parent.size.y;
                    var nodeTop = position.y;
                    var nodeBottom = parentHeight - (nodeTop+node.size.y);
                    style.top = new StyleLength(new Length(nodeTop, LengthUnit.Pixel));
                    style.bottom = new StyleLength(new Length(nodeBottom, LengthUnit.Pixel));
                    style.height = new StyleLength(StyleKeyword.Auto);
                }
                else if (constraintY == Vertical.Scale)
                {
                    var parentHeight = parent.size.y;
                    var nodeTop = position.y;
                    var nodeBottom = parentHeight - (nodeTop+node.size.y);
                    var topPercentage = (nodeTop * 100.0f) / parentHeight;
                    var bottomPercentage = (nodeBottom * 100.0f) / parentHeight;
                    style.top = new StyleLength(new Length(topPercentage, LengthUnit.Percent));
                    style.bottom = new StyleLength(new Length(bottomPercentage, LengthUnit.Percent));
                }
            }
            /*positioning and size*/
            
            /*color*/
            var fills = node.fills;
            if (fills.Length > 0)
            {
                //only getting the base color
                var solidColor = (SolidPaint) fills[0];
                var fillColorBlended = solidColor.color;
                style.backgroundColor = new StyleColor(fillColorBlended);
            }
            /*color*/
            
            /*shaping*/
            var cornerRadius = node.cornerRadius;
            var cornerRadii = node.rectangleCornerRadii;
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
            
            var strokes = node.strokes;
            if (strokes.Length > 0)
            {
                var strokeWeight = node.strokeWeight;
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
            /*shaping*/
            
            // Figma transform pivot is located on the top left.
            style.transformOrigin = new StyleTransformOrigin(new TransformOrigin(Length.Percent(0f), Length.Percent(0f), 0.0f));
        }
        
    }
    
}