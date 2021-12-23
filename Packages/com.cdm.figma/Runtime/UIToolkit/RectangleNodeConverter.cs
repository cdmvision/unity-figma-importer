using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public class RectangleNodeConverter : NodeConverter<RectangleNode>
    {
        public override NodeElement Convert(NodeElement parentElement, Node node, NodeConvertArgs args)
        {
            var rectangleNode = (RectangleNode) node;
            var nodeElement = VectorNodeConverter.Convert(parentElement, rectangleNode, args);
            BuildStyle(rectangleNode, nodeElement.inlineStyle);
            return nodeElement;
        }

        private void BuildStyle(RectangleNode node, Style style)
        {
            AddCorners(node, style);
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
        }
    }
}