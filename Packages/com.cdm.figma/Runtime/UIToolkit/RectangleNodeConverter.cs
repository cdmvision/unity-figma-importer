using System;
using System.Xml.Linq;
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
            var styleAttributes = BuildStyle(rectangleNode);
            var rectangleElement =
                new XElement(args.namespaces.engine + nameof(VisualElement),
                    new XAttribute("name", rectangleNode.name),
                    new XAttribute("style", styleAttributes));

            Debug.Log($"Rectangle XML element: {rectangleElement}");
            return rectangleElement;
        }

        private string BuildStyle(RectangleNode node)
        {
            string styleAttributes = "";
            
            /*positioning and size*/
            
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

                foreach (var stroke in strokes)
                {
                    //Probably we will use only one color (for stroke color), but it can change in the future.
                    //TODO: Implement when strokes have more than one stroke color.
                    styleAttributes += "border-color: rgba(";
                    var strokeColor = stroke.color;
                    styleAttributes += strokeColor.r * 255 + "," + strokeColor.g * 255 + "," + strokeColor.b * 255 +
                                       "," + strokeColor.a + "); ";
                }
                
                //TODO: Implement dashed strokes
            }
            /*shaping*/
            
            /*color*/
            var fills = node.fills;
            foreach (var fill in fills)
            {
                //Probably we will use only one color (for background color), but it can change in the future,
                //If fills have more than one fill color, then Blend Type must be checked. 
                //TODO: Implement when fills have more than one fill color.
                styleAttributes += "background-color: rgba(";
                var bgColor = fill.color;
                //Figma stores rgb values in [0,1], change it to [0,255] (except alpha).
                styleAttributes += bgColor.r*255 + "," + bgColor.g*255 + "," + bgColor.b*255 + "," + bgColor.a + "); ";
            }
            /*color*/
            
            /*effects*/
            var effects = node.effects;
            //TODO: Implement effects, find equivalent (uxml) to them.
            /*effects*/
 
            return styleAttributes;
        }
    }
}