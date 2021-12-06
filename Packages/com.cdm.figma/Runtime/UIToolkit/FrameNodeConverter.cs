using System.Xml.Linq;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit.UIToolkit
{
    public class FrameNodeConverter : NodeConverter<FrameNode>
    {
        public override NodeElement Convert(Node node, NodeConvertArgs args)
        {
            var frameNode = (FrameNode) node;
            var nodeElement = NodeElement.New<VisualElement>(frameNode, args);
            BuildStyle(frameNode, nodeElement.inlineStyle);
            var children = frameNode.children;
            for (int child = 0; child < children.Length; child++)
            {
                if (args.importer.TryConvertNode(children[child], args, out var childElement))
                {
                    if (child != children.Length - 1)
                    {
                        if (frameNode.layoutMode == LayoutMode.Horizontal)
                        {
                            childElement.inlineStyle.marginRight =
                                new StyleLength(new Length(frameNode.itemSpacing, LengthUnit.Pixel));
                        }
                        else if (frameNode.layoutMode == LayoutMode.Vertical)
                        {
                            childElement.inlineStyle.marginBottom =
                                new StyleLength(new Length(frameNode.itemSpacing, LengthUnit.Pixel));
                        }
                    }
                    nodeElement.AddChild(childElement);
                }
            }
            return nodeElement;
        }

        private void BuildStyle(FrameNode node, Style style)
        {
            style.position = new StyleEnum<Position>(Position.Relative);
            style.width = new StyleLength(new Length(node.size.x, LengthUnit.Pixel));
            style.height = new StyleLength(new Length(node.size.y, LengthUnit.Pixel));

            var layoutMode = node.layoutMode;
            //check if there is layout
            if (layoutMode != LayoutMode.None)
            {
                style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                if (layoutMode == LayoutMode.Horizontal)
                {
                    style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                }
                else
                {
                    style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
                }
                if (node.primaryAxisAlignItems == PrimaryAxisAlignItems.Min)
                {
                    style.justifyContent = new StyleEnum<Justify>(Justify.FlexStart);
                }
                else if (node.primaryAxisAlignItems == PrimaryAxisAlignItems.Max)
                {
                    style.justifyContent = new StyleEnum<Justify>?(Justify.FlexEnd);
                }
                else if(node.primaryAxisAlignItems == PrimaryAxisAlignItems.Center)
                {
                    style.justifyContent = new StyleEnum<Justify>?(Justify.Center);
                }
                else if(node.primaryAxisAlignItems == PrimaryAxisAlignItems.SpaceBetween)
                {
                    style.justifyContent = new StyleEnum<Justify>?(Justify.SpaceBetween);
                }
                
                if (node.counterAxisAlignItems == CounterAxisAlignItems.Min)
                {
                    style.alignItems = new StyleEnum<Align>(Align.FlexStart);
                }
                else if (node.counterAxisAlignItems == CounterAxisAlignItems.Max)
                {
                    style.alignItems = new StyleEnum<Align>(Align.FlexEnd);
                }
                else if(node.counterAxisAlignItems == CounterAxisAlignItems.Center)
                {
                    style.alignItems = new StyleEnum<Align>(Align.Center);
                }
            }
            
            /*padding*/
            style.paddingTop = new StyleLength(new Length(node.paddingTop, LengthUnit.Pixel));
            style.paddingLeft = new StyleLength(new Length(node.paddingLeft, LengthUnit.Pixel));
            style.paddingBottom = new StyleLength(new Length(node.paddingBottom, LengthUnit.Pixel));
            style.paddingRight = new StyleLength(new Length(node.paddingRight, LengthUnit.Pixel));
            /*padding*/
            
            /*color*/
            var fills = node.fills;
            if (fills.Count > 0)
            {
                var solidColor = (SolidPaint) fills[0];
                style.backgroundColor = new StyleColor(solidColor.color);
            }
            /*color*/
            
            // Figma transform pivot is located on the top left.
            style.transformOrigin = new StyleTransformOrigin(new TransformOrigin(Length.Percent(0f), Length.Percent(0f), 0.0f));
        }
    }
}