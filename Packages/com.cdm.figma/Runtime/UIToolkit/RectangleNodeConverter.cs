using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(RectangleNodeConverter), menuName = AssetMenuRoot + "Rectangle", order = 20)]
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
            /*positioning and size*/
            //TODO: Implement responsive (Figma design should change for this)
            styleAttributes += "position: absolute; ";
            var absoluteBoundingBox = node.absoluteBoundingBox;
            var relativeTransform = node.relativeTransform;
            var relativeTranformValues = relativeTransform.values;
            var m00 = relativeTranformValues[0][0];
            var m10 = relativeTranformValues[1][0];
            var rotation = Math.Atan2(m10, m00);
            var rotationDeg = (180 / Math.PI) * rotation;
            if (rotationDeg != 0)
            {
                var sizeX = node.size.x;
                var sizeY = node.size.y;
                styleAttributes += "rotate: " + rotationDeg + "deg; ";
                styleAttributes += "width: " + sizeX + "px; ";
                styleAttributes += "height: " + sizeY + "px; ";
            }
            else
            {
                var width = absoluteBoundingBox.width;
                styleAttributes += "width: " + width + "px; ";
                var height = absoluteBoundingBox.height;
                styleAttributes += "height: " + height + "px; ";
            }
            
            var xValue = absoluteBoundingBox.x;
            var yValue = absoluteBoundingBox.y;
            var constraints = node.constraints;
            var constraintX = constraints.horizontal;
            //Constraints are stored as "Top" or "Left" etc..
            //Using toLower because uxml expects "top" or "left" etc...
            styleAttributes += constraintX.ToString().ToLower() + ": " + xValue + "px; ";   
            var constraintY = constraints.vertical;
            styleAttributes += constraintY.ToString().ToLower() + ": " + yValue + "px; ";
            /*positioning and size*/
            
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
                styleAttributes += "border-color: rgba(";
                var strokeColorBlended = strokeColor.color;
                styleAttributes += strokeColorBlended.r*255 + "," + strokeColorBlended.g*255 + "," + strokeColorBlended.b*255 + "," + strokeColorBlended.a + "); ";
                //TODO: Implement dashed strokes
            }
            /*shaping*/
            
            /*color*/
            var fills = node.fills;
            if (fills.Count > 0)
            {
                var fillColor = BlendColor(fills);
                styleAttributes += "background-color: rgba(";
                var fillColorBlended = fillColor.color;
                
                //Figma stores rgb values in [0,1], change it to [0,255] (except alpha).
                styleAttributes += fillColorBlended.r*255 + "," + fillColorBlended.g*255 + "," + fillColorBlended.b*255 + "," + fillColorBlended.a + "); ";
            }
            /*color*/
            
            /*effects*/
            var effects = node.effects;
            //TODO: Implement effects, find equivalent (uxml) to them.
            /*effects*/
 
            return styleAttributes;
        }

        private Paint BlendColor(List<Paint> paints)
        {
            //TODO: Change blend mode.
            
            if (paints.Count == 1)
            {
                return paints[0]; //base color
            }

            for (int i = 0; i < paints.Count-1; ++i)
            {
                paints[i+1] = BlendTwoColors(paints[i+1], paints[i]);
            }
            return paints[paints.Count-1];
        }

        private Paint BlendTwoColors(Paint addedColor, Paint baseColor)
        {
            Paint mix = new Paint();
            mix.color = new Color();
            mix.color.a = 1 - (1 - addedColor.opacity) * (1 - baseColor.opacity); 
            mix.color.r = (float) Math.Round((addedColor.color.r * addedColor.opacity / mix.color.a) 
                                             + (baseColor.color.r * baseColor.opacity * (1 - addedColor.opacity) / mix.color.a)); 
            mix.color.g = (float) Math.Round((addedColor.color.g * addedColor.opacity / mix.color.a) 
                                             + (baseColor.color.g * baseColor.opacity * (1 - addedColor.opacity) / mix.color.a)); 
            mix.color.b = (float) Math.Round((addedColor.color.b * addedColor.opacity / mix.color.a) 
                                             + (baseColor.color.b * baseColor.opacity * (1 - addedColor.opacity) / mix.color.a));
            return mix;
        }
    }
    
}