using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(RectangleNodeConverter), 
        menuName = AssetMenuRoot + "Rectangle", order = AssetMenuOrder)]
    public class RectangleNodeConverter : NodeConverter<RectangleNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            var rectangleNode = (RectangleNode) node;
            var styleAttributes = BuildStyle(rectangleNode, args);
            return XmlFactory.NewElement<VisualElement>(rectangleNode, args).Style(styleAttributes);
        }

        private string BuildStyle(RectangleNode node, NodeConvertArgs args)
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
            
            /*color*/
            var fills = node.fills;
            if (fills.Count > 0)
            {
                var fillColor = BlendColor(fills);
                var solidColor = (SolidPaint) fillColor;
                var fillColorBlended = solidColor.color;
                styleAttributes += "background-color: " + fillColorBlended.ToString("rgba") + "; ";
            }
            /*color*/
            
            /*shaping*/
            var cornerRadius = node.cornerRadius;
            var cornerRadii = node.rectangleCornerRadii;
            //If element has 4 different radius for each corner, use this
            if (cornerRadii != null)
            {
                styleAttributes += "border-radius: ";
                styleAttributes += cornerRadii[0] + "px " + 
                                   cornerRadii[1] + "px " + 
                                   cornerRadii[2] + "px " + 
                                   cornerRadii[3] + "px; "; 
            }
            //Element does not have 4 different radius for each corner...
            else
            {
                //Does it have cornerRadius?
                if (cornerRadius.HasValue)
                {
                    styleAttributes += "border-radius: " + cornerRadius + "px; ";
                }
            }
            
            var strokes = node.strokes;
            if (strokes.Count > 0)
            {
                var strokeWeight = node.strokeWeight;
                if (strokeWeight.HasValue)
                {
                    styleAttributes += "border-width: " + strokeWeight + "px; ";
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

        private Paint BlendColor(List<Paint> paints)
        {
            if (paints.Count > 1)
            {
                Debug.LogWarning("Found node with multiple colors. Returning base color, color blending is not available.");
            }
            return paints[0]; //base color
        }
    }
    
}