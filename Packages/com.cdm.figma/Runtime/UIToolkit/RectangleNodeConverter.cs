using System;
using System.Collections.Generic;
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
            
            /*positioning and size*/
            //TODO: Implement responsive 
            styleAttributes += "position: absolute; ";
            var relativeTransform = node.relativeTransform;
            var position = relativeTransform.GetPosition();
            var rotation = relativeTransform.GetRotationAngle();
            styleAttributes += "width: " + node.size.x + "px; ";
            styleAttributes += "height: " + node.size.y + "px; ";
            if (rotation != 0.0f)
            {
                styleAttributes += "rotate: " + rotation + "deg; ";
            }
            var constraintX = node.constraints.horizontal;
            styleAttributes += constraintX.ToString().ToLower() + ": " + position.x + "px; ";   //Left, Top... ==> left, top...
            var constraintY = node.constraints.vertical;
            styleAttributes += constraintY.ToString().ToLower() + ": " + position.y + "px; ";
            /*positioning and size*/
            
            /*color*/
            var fills = node.fills;
            if (fills.Count > 0)
            {
                var fillColor = BlendColor(fills);
                var fillColorBlended = fillColor.color;
                styleAttributes += "background-color: " + fillColorBlended.ToString().Substring(0,7) + "; ";
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
                var strokeColorBlended = strokeColor.color;
                styleAttributes += "border-color: " + strokeColorBlended.ToString().Substring(0,7) + "; ";
            }
            /*shaping*/
            
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