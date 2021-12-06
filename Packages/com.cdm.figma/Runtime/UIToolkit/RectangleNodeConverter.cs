using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
            var element = NodeElement.New<VisualElement>(rectangleNode, args);
            BuildStyle(rectangleNode, element.inlineStyle);
            return element;
        }

        private void BuildStyle(RectangleNode node, Style style)
        {
            string styleAttributes = "";
            GroupNode parent = new GroupNode();
            
            if (node.hasParent)
            {
                if (node.parent.type == "FRAME" || node.parent.type == "GROUP")
                {
                    if (node.parent.type == "FRAME")
                        parent = (FrameNode) node.parent;
                    else
                        parent = (GroupNode) node.parent;
                }
            }

            /*positioning and size*/
            if (parent.layoutMode != LayoutMode.None)
            {
                styleAttributes += "position: relative; ";
                if (node.layoutAlign == LayoutAlign.Stretch)
                {
                    styleAttributes += "align-self: stretch; ";
                    if (parent.layoutMode == LayoutMode.Horizontal)
                    {
                        styleAttributes += "height: auto; ";
                        styleAttributes += "width: " + node.size.x + "px; ";
                    }
                    else
                    {
                        styleAttributes += "width: auto; ";
                        styleAttributes += "height: " + node.size.y + "px; ";
                    }
                }
                else
                {
                    styleAttributes += "width: " + node.size.x + "px; ";
                    styleAttributes += "height: " + node.size.y + "px; ";
                }
                
                var relativeTransform = node.relativeTransform;
                var rotation = relativeTransform.GetRotationAngle();
                if (rotation != 0.0f)
                {
                    styleAttributes += "rotate: " + rotation + "deg; ";
                }
                styleAttributes += "flex-grow: " + node.layoutGrow + "; ";
                
                
                //adding margin, don't add margin to the last element!
                var parentsChildren = parent.GetChildren();
                if (parentsChildren.ElementAt(parentsChildren.Length - 1) != node)
                {
                    if (parent.layoutMode == LayoutMode.Horizontal)
                    {
                        styleAttributes += "margin-right: " + parent.itemSpacing + "; ";
                    }
                    else
                    {
                        styleAttributes += "margin-bottom: " + parent.itemSpacing + "; ";
                    }
                }
            }

            else
            {
                styleAttributes += "position: absolute; ";
                var relativeTransform = node.relativeTransform;
                var position = relativeTransform.GetPosition();
                var rotation = relativeTransform.GetRotationAngle();
                if (rotation != 0.0f)
                {
                    styleAttributes += "rotate: " + rotation + "deg; ";
                }
                var constraintX = node.constraints.horizontal;
                if (constraintX == Horizontal.Center)
                {
                    //handle width
                    //calc() function?
                }
                else if (constraintX == Horizontal.Left)
                {
                    styleAttributes += "width: " + node.size.x + "px; ";
                    styleAttributes += "left: " + position.x + "px; ";
                }
                else if (constraintX == Horizontal.Right)
                {
                    styleAttributes += "width: " + node.size.x + "px; ";
                    styleAttributes += "right: " + position.x + "px; ";
                }
                else if (constraintX == Horizontal.LeftRight)
                {
                    var parentWidth = parent.size.x;
                    var nodeLeft = node.relativeTransform.GetPosition().x;
                    var nodeRight = parentWidth - (nodeLeft+node.size.x);
                    styleAttributes += "left: " + nodeLeft + "px; ";
                    styleAttributes += "right: " + nodeRight + "px; ";
                    styleAttributes += "width: auto; ";
                }
                else if (constraintX == Horizontal.Scale)
                {
                    var parentWidth = parent.size.x;
                    var nodeLeft = node.relativeTransform.GetPosition().x;
                    var nodeRight = parentWidth - (nodeLeft+node.size.x);
                    var leftPercentage = (nodeLeft * 100.0f) / parentWidth;
                    var rightPercentage = (nodeRight * 100.0f) / parentWidth;
                    styleAttributes += "left: " + leftPercentage + "%; ";
                    styleAttributes += "right: " + rightPercentage + "%; ";
                }
                
                var constraintY = node.constraints.vertical;
                if (constraintY == Vertical.Center)
                {
                    //handle height
                    //calc() function?
                }
                else if (constraintY == Vertical.Top)
                {
                    styleAttributes += "height: " + node.size.y + "px; ";
                    styleAttributes += "top: " + position.y + "px; ";
                }
                else if (constraintY == Vertical.Bottom)
                {
                    styleAttributes += "height: " + node.size.y + "px; ";
                    styleAttributes += "bottom: " + position.y + "px; ";
                }
                else if (constraintY == Vertical.TopBottom)
                {
                    var parentHeight = parent.size.y;
                    var nodeTop = node.relativeTransform.GetPosition().y;
                    var nodeBottom = parentHeight - (nodeTop+node.size.y);
                    styleAttributes += "top: " + nodeTop + "px; ";
                    styleAttributes += "bottom: " + nodeBottom + "px; ";
                    styleAttributes += "height: auto; ";
                }
                else if (constraintY == Vertical.Scale)
                {
                    var parentHeight = parent.size.y;
                    var nodeTop = node.relativeTransform.GetPosition().y;
                    var nodeBottom = parentHeight - (nodeTop+node.size.y);
                    var topPercentage = (nodeTop * 100.0f) / parentHeight;
                    var bottomPercentage = (nodeBottom * 100.0f) / parentHeight;
                    styleAttributes += "top: " + topPercentage + "%; ";
                    styleAttributes += "bottom: " + bottomPercentage + "%; ";
                }
            }

            /*positioning and size*/
            
            // TODO: //style.translate = new StyleTranslate(new Translate())
            // TODO: var constraintX = node.constraints.horizontal;
            // TODO: styleAttributes += constraintX.ToString().ToLower() + ": " + position.x + "px; ";   //Left, Top... ==> left, top...
            // TODO: var constraintY = node.constraints.vertical;
            // TODO: styleAttributes += constraintY.ToString().ToLower() + ": " + position.y + "px; ";
            // TODO: /*positioning and size*/
            
            var fills = node.fills;
            if (fills.Length > 0)
            {
                if (fills[0] is SolidPaint solidColor)
                {
                    style.backgroundColor = new StyleColor(solidColor.color);
                }
            }
            
            // Border radius.
            if (node.cornerRadius.HasValue || node.rectangleCornerRadii != null)
            {
                style.borderBottomLeftRadius = new StyleLength(new Length(node.bottomLeftRadius, LengthUnit.Pixel));
                style.borderBottomRightRadius = new StyleLength(new Length(node.bottomRightRadius, LengthUnit.Pixel));
                style.borderTopLeftRadius = new StyleLength(new Length(node.topLeftRadius, LengthUnit.Pixel));
                style.borderTopRightRadius = new StyleLength(new Length(node.topRightRadius, LengthUnit.Pixel));
            }

            // Border width and color.
            var strokes = node.strokes;
            if (strokes.Length > 0)
            {
                var strokeWeight = node.strokeWeight;
                if (strokeWeight.HasValue)
                {
                    style.borderBottomWidth = new StyleFloat(strokeWeight.Value);
                    style.borderLeftWidth = new StyleFloat(strokeWeight.Value);
                    style.borderRightWidth = new StyleFloat(strokeWeight.Value);
                    style.borderTopWidth = new StyleFloat(strokeWeight.Value);
                }
                var strokeColor = BlendColor(strokes);
                var solidColor = (SolidPaint) strokeColor;
                var strokeColorBlended = solidColor.color;
                styleAttributes += "border-color: " + strokeColorBlended.ToString("rgba") + "; ";
            }
            /*shaping*/
            
            // Figma transform pivot is located on the top left.
            styleAttributes += "transform-origin: left top; ";
            
            return styleAttributes;
        }

                if (strokes[0] is SolidPaint strokeColor)
                {
                    style.borderBottomColor = new StyleColor(strokeColor.color);
                    style.borderLeftColor = new StyleColor(strokeColor.color);
                    style.borderRightColor = new StyleColor(strokeColor.color);
                    style.borderTopColor = new StyleColor(strokeColor.color);
                }
            }
        }
    }
}