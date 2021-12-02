using System.Xml.Linq;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
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
            if (fills.Length > 0)
            {
                if (fills[0] is SolidPaint solidColor)
                {
                    var fillColorBlended = solidColor.color;
                    styleAttributes += "background-color: " + fillColorBlended.ToString("rgba") + "; ";     
                }
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
            if (strokes.Length > 0)
            {
                var strokeWeight = node.strokeWeight;
                if (strokeWeight.HasValue)
                {
                    styleAttributes += "border-width: " + strokeWeight + "px; ";
                }

                if (strokes[0] is SolidPaint strokeColor)
                {
                    var strokeColorBlended = strokeColor.color;
                    styleAttributes += "border-color: " + strokeColorBlended.ToString("rgba") + "; ";    
                }
            }
            /*shaping*/
            
            // Figma transform pivot is located on the top left.
            styleAttributes += "transform-origin: left top;";

            return styleAttributes;
        }
        
    }
    
}