using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class RectangleNodeConverter : NodeConverter<RectangleNode>
    {
        public override NodeElement Convert(Node node, NodeConvertArgs args)
        {
            var rectangleNode = (RectangleNode) node;
            var nodeElement = VectorNodeConverter.Convert(rectangleNode, args);
            BuildStyle(rectangleNode, nodeElement.inlineStyle);
            return nodeElement;
        }

        private void BuildStyle(RectangleNode node, Style style)
        {
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
            style.transformOrigin =
                new StyleTransformOrigin(new TransformOrigin(Length.Percent(0f), Length.Percent(0f), 0.0f));
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
                    style.borderTopLeftRadius = new StyleLength(new Length((float) cornerRadius, LengthUnit.Pixel));
                    style.borderTopRightRadius = new StyleLength(new Length((float) cornerRadius, LengthUnit.Pixel));
                    style.borderBottomRightRadius = new StyleLength(new Length((float) cornerRadius, LengthUnit.Pixel));
                    style.borderBottomLeftRadius = new StyleLength(new Length((float) cornerRadius, LengthUnit.Pixel));
                }
            }

            var strokes = rectangleNode.strokes;
            if (strokes.Length > 0)
            {
                var strokeWeight = rectangleNode.strokeWeight;
                if (strokeWeight.HasValue)
                {
                    style.borderTopWidth = new StyleFloat((float) strokeWeight);
                    style.borderLeftWidth = new StyleFloat((float) strokeWeight);
                    style.borderBottomWidth = new StyleFloat((float) strokeWeight);
                    style.borderRightWidth = new StyleFloat((float) strokeWeight);
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