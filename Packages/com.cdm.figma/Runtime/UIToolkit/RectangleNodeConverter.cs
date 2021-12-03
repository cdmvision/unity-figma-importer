using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class RectangleNodeConverter : NodeConverter<RectangleNode>
    {
        public override NodeData Convert(Node node, NodeConvertArgs args)
        {
            var rectangleNode = (RectangleNode) node;
            var data = NodeData.New<VisualElement>(rectangleNode, args);
            BuildStyle(rectangleNode, data.style);
            data.UpdateStyle();
            return data;
        }

        private void BuildStyle(RectangleNode node, Style style)
        {
            /*positioning and size*/
            //TODO: Implement responsive 

            style.position = new StyleEnum<Position>(Position.Absolute);
            style.width = new StyleLength(node.size.x);
            style.height = new StyleLength(node.size.y);

            var relativeTransform = node.relativeTransform;
            var position = relativeTransform.GetPosition();
            var rotation = relativeTransform.GetRotationAngle();
            if (rotation != 0.0f)
            {
                style.rotate = new StyleRotate(new Rotate(new Angle(rotation, AngleUnit.Degree)));
            }
            
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