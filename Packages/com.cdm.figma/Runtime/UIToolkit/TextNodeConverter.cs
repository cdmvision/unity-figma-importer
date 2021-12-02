using System;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    /// <summary>
    /// Missing features:
    /// - paragraphIndent
    /// - listSpacing
    /// - textCase
    /// - textDecoration
    /// - lineHeightPx
    /// - lineHeightUnit
    /// - lineHeightPercentFontSize
    /// - TextAlignHorizontal.Justified
    ///
    /// Unity does not support individual character coloring:
    /// - node.styleOverrideTable
    /// - node.characterStyleOverrides
    /// </summary>
    [CreateAssetMenu(fileName = nameof(TextNodeConverter), menuName = AssetMenuRoot + "Text", order = AssetMenuOrder)]
    public class TextNodeConverter : NodeConverter<TextNode>
    {
        public override XElement Convert(Node node, NodeConvertArgs args)
        {
            var textNode = (TextNode) node;
            var style = BuildStyle(textNode, args);
            var label = XmlFactory.NewElement<Label>(node, args).Style(style);
            label.SetAttributeValue(nameof(Label.text), textNode.characters);
            return label;
        }

        private static string BuildStyle(TextNode node, NodeConvertArgs args)
        {
            var style = new StringBuilder();

            var fontName = FontSource.GetFontName(node.style.fontFamily, node.style.fontWeight, node.style.italic);
            
            if (args.file.TryGetFontUrl(fontName, out var fontUrl))
            {
                // We set font type directly based on family, weight and the italic information.
                // So, we do not need to add font weight or style.
                style.Append($"-unity-font-definition: {fontUrl}; ");
            }
            else
            {
                Debug.LogWarning($"{fontName} could not be found.");
            }

            if (node.fills != null && node.fills.Length > 0)
            {
                var solidPaint = node.fills[0] as SolidPaint;
                if (solidPaint != null)
                {
                    style.Append($"color: {solidPaint.color.ToString("rgba")};");        
                }
            }

            style.Append($"font-size: {node.style.fontSize};");
            style.Append($"letter-spacing: {node.style.letterSpacing} px;");
            style.Append($"-unity-paragraph-spacing: {node.style.paragraphSpacing} px;");
            
            style.Append(GetTextSize(node));
            style.Append(GetTextAlignStyle(node.style.textAlignHorizontal, node.style.textAlignVertical));
            
            return style.ToString();
        }

        private static string GetTextSize(TextNode node)
        {
            switch (node.style.textAutoResize)
            {
                case TextAutoResize.None:
                    return $"width: {node.size.x} px; height: {node.size.y} px;";
                case TextAutoResize.Height:
                    return $"width: {node.size.x} px; height: auto;";
                case TextAutoResize.WidthAndHeight:
                    return $"width: auto; height: auto;";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private static string GetTextAlignStyle(TextAlignHorizontal horizontal, TextAlignVertical vertical)
        {
            return $"-unity-text-align: {GetTextAlignVerticalStyle(vertical)}-{GetTextAlignHorizontalStyle(horizontal)};";
        }

        private static string GetTextAlignHorizontalStyle(TextAlignHorizontal horizontal)
        {
            switch (horizontal)
            {
                case TextAlignHorizontal.Center:
                    return "center";
                case TextAlignHorizontal.Justified:
                    return "left";    // TODO: ???
                case TextAlignHorizontal.Left:
                    return "left";
                case TextAlignHorizontal.Right:
                    return "right";
                default:
                    throw new ArgumentOutOfRangeException(nameof(horizontal), horizontal, null);
            }
        }
        
        private static string GetTextAlignVerticalStyle(TextAlignVertical vertical)
        {
            switch (vertical)
            {
                case TextAlignVertical.Bottom:
                    return "lower";
                case TextAlignVertical.Center:
                    return "middle";
                case TextAlignVertical.Top:
                    return "upper";
                default:
                    throw new ArgumentOutOfRangeException(nameof(vertical), vertical, null);
            }
        }
    }
}