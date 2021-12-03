using System.Xml.Linq;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(FrameNodeConverter), 
        menuName = AssetMenuRoot + "Frame", order = AssetMenuOrder)]
    public class FrameNodeConverter : NodeConverter<FrameNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            var frameNode = (FrameNode) node;
            var styleAttributes = BuildStyle(frameNode, args);
            var parentXml = XmlFactory.NewElement<VisualElement>(frameNode, args).Style(styleAttributes);
            var children = frameNode.children;
            foreach (var child in children)
            {
                if (args.importer.TryConvertNode(child, args, out var childElement))
                {
                    parentXml.Add(childElement);
                }
            }
            return parentXml;
        }

        private string BuildStyle(FrameNode node, NodeConvertArgs args)
        {
            string styleAttributes = "";
            
            styleAttributes += "position: relative; ";
            styleAttributes += "width: " + node.size.x + "px; ";
            styleAttributes += "height: " + node.size.y + "px; ";
            
            var layoutMode = node.layoutMode;
            //check if there is layout
            if (layoutMode != LayoutMode.None)
            {
                styleAttributes += "display: flex; ";
                if (layoutMode == LayoutMode.Horizontal)
                {
                    styleAttributes += "flex-direction: row; ";
                }
                else
                {
                    styleAttributes += "flex-direction: column; ";
                }
                if (node.primaryAxisAlignItems == PrimaryAxisAlignItems.Min)
                {
                    styleAttributes += "justify-content: flex-start; ";
                }
                else if (node.primaryAxisAlignItems == PrimaryAxisAlignItems.Max)
                {
                    styleAttributes += "justify-content: flex-end; ";
                }
                else if(node.primaryAxisAlignItems == PrimaryAxisAlignItems.Center)
                {
                    styleAttributes += "justify-content: center; ";
                }
                else if(node.primaryAxisAlignItems == PrimaryAxisAlignItems.SpaceBetween)
                {
                    styleAttributes += "justify-content: space-between; ";
                }
                
                if (node.counterAxisAlignItems == CounterAxisAlignItems.Min)
                {
                    styleAttributes += "align-items: flex-start; ";
                }
                else if (node.counterAxisAlignItems == CounterAxisAlignItems.Max)
                {
                    styleAttributes += "align-items: flex-end; ";
                }
                else if(node.counterAxisAlignItems == CounterAxisAlignItems.Center)
                {
                    styleAttributes += "align-items: center; ";
                }
            }
            
            /*padding*/
            styleAttributes += "padding-bottom: " + node.paddingBottom + "; ";
            styleAttributes += "padding-top: " + node.paddingTop + "; ";
            styleAttributes += "padding-left: " + node.paddingLeft + "; ";
            styleAttributes += "padding-right: " + node.paddingRight + "; ";
            /*padding*/
            
            /*color*/
            var fills = node.fills;
            if (fills.Count > 0)
            {
                var solidColor = (SolidPaint) fills[0];
                styleAttributes += "background-color: " + solidColor.color.ToString("rgba") + "; ";
            }
            /*color*/
            
            // Figma transform pivot is located on the top left.
            styleAttributes += "transform-origin: left top; ";
            
            return styleAttributes;
        }
    }
}