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
            bool positionSet = false;
            string styleAttributes = "";
            
            /*positioning and size*/
            //TODO: Implement responsive 
            if (node.hasParent)
            {
                if (node.parent.type == "FRAME" || node.parent.type == "GROUP")
                {
                    GroupNode parent;
                    if (node.parent.type == "FRAME")
                        parent = (FrameNode) node.parent;
                    else
                        parent = (GroupNode) node.parent;
                    
                    if (parent.layoutMode != LayoutMode.None)
                    {
                        styleAttributes += "position: relative; ";
                        styleAttributes += "width: " + node.size.x + "px; ";
                        styleAttributes += "height: " + node.size.y + "px; ";
                        var relativeTransform = node.relativeTransform;
                        var rotation = relativeTransform.GetRotationAngle();
                        if (rotation != 0.0f)
                        {
                            styleAttributes += "rotate: " + rotation + "deg; ";
                        }
                        styleAttributes += "flex-grow: " + node.layoutGrow + "; ";
                        if (node.layoutAlign == LayoutAlign.Stretch)
                        {
                            styleAttributes += "align-self: stretch; ";
                        }
                        
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
                        
                        positionSet = true;
                    }
                }
            }

            if (!positionSet)
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
                    styleAttributes += "height: " + node.size.y + "px; ";
                    styleAttributes += "left: " + position.x + "px; ";
                }
                if (constraintX == Horizontal.Right)
                {
                    styleAttributes += "width: " + node.size.x + "px; ";
                    styleAttributes += "height: " + node.size.y + "px; ";
                    styleAttributes += "right: " + position.x + "px; ";
                }
                if (constraintX == Horizontal.LeftRight)
                {
                    //handle width
                }
                if (constraintX == Horizontal.Scale)
                {
                    //handle width
                }
                
                var constraintY = node.constraints.vertical;
                if (constraintY == Vertical.Center)
                {
                    //handle height
                    //calc() function?
                }
                else if (constraintY == Vertical.Top)
                {
                    styleAttributes += "width: " + node.size.x + "px; ";
                    styleAttributes += "height: " + node.size.y + "px; ";
                    styleAttributes += "top: " + position.y + "px; ";
                }
                if (constraintY == Vertical.Bottom)
                {
                    styleAttributes += "width: " + node.size.x + "px; ";
                    styleAttributes += "height: " + node.size.y + "px; ";
                    styleAttributes += "bottom: " + position.y + "px; ";
                }
                if (constraintY == Vertical.TopBottom)
                {
                    //handle height
                }
                if (constraintY == Vertical.Scale)
                {
                    //handle height
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