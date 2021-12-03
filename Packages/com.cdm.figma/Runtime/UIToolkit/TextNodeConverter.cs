using System;
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
    public class TextNodeConverter : NodeConverter<TextNode>
    {
        public override NodeElement Convert(Node node, NodeConvertArgs args)
        {
            var textNode = (TextNode) node;
            var element = NodeElement.New<Label>(node, args);
            element.value.SetAttributeValue(nameof(Label.text), textNode.characters);
            BuildStyle(textNode, args, element.style);
            element.UpdateStyle();
            return element;
        }

        private static void BuildStyle(TextNode node, NodeConvertArgs args, Style style)
        {
            var fontName = FontSource.GetFontName(node.style.fontFamily, node.style.fontWeight, node.style.italic);
            
            if (args.file.TryGetFont(fontName, out var font))
            {
                // We set font type directly based on family, weight and the italic information.
                // So, we do not need to add font weight or style.
                style.unityFontDefinition = new StyleFontDefinition(font);
            }
            else
            {
                Debug.LogWarning($"{fontName} could not be found.");
            }

            if (node.fills != null && node.fills.Length > 0)
            {
                if (node.fills[0] is SolidPaint solidPaint)
                {
                    style.color = new StyleColor(solidPaint.color);
                }
            }

            style.fontSize = new StyleLength(node.style.fontSize);
            style.letterSpacing = new StyleLength(node.style.letterSpacing);
            style.unityParagraphSpacing = new StyleLength(node.style.paragraphSpacing);
            
            SetTextSize(node, style);
            SetTextAlignStyle(node.style.textAlignHorizontal, node.style.textAlignVertical, style);
        }

        private static void SetTextSize(TextNode node, Style style)
        {
            switch (node.style.textAutoResize)
            {
                case TextAutoResize.None:
                    style.width = new StyleLength(node.size.x);
                    style.height = new StyleLength(node.size.y);
                    break;
                case TextAutoResize.Height:
                    style.width = new StyleLength(node.size.x);
                    style.height = new StyleLength(StyleKeyword.Auto);
                    break;
                case TextAutoResize.WidthAndHeight:
                    style.width = new StyleLength(StyleKeyword.Auto);
                    style.height = new StyleLength(StyleKeyword.Auto);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SetTextAlignStyle(TextAlignHorizontal horizontal, TextAlignVertical vertical, Style style)
        {
            switch (vertical)
            {
                case TextAlignVertical.Bottom:
                    switch (horizontal)
                    {
                        case TextAlignHorizontal.Center:
                            style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.LowerCenter);
                            break;
                        case TextAlignHorizontal.Justified: // TODO: ???
                            style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.LowerLeft);
                            break;
                        case TextAlignHorizontal.Left:
                            style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.LowerLeft);
                            break;
                        case TextAlignHorizontal.Right:
                            style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.LowerRight);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(horizontal), horizontal, null);
                    }
                    break;
                case TextAlignVertical.Center:
                    switch (horizontal)
                    {
                        case TextAlignHorizontal.Center:
                            style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
                            break;
                        case TextAlignHorizontal.Justified: // TODO: ???
                            style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleLeft);
                            break;
                        case TextAlignHorizontal.Left:
                            style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleLeft);
                            break;
                        case TextAlignHorizontal.Right:
                            style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleRight);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(horizontal), horizontal, null);
                    }
                    break;
                case TextAlignVertical.Top:
                    switch (horizontal)
                    {
                        case TextAlignHorizontal.Center:
                            style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.UpperCenter);
                            break;
                        case TextAlignHorizontal.Justified: // TODO: ???
                            style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.UpperLeft);
                            break;
                        case TextAlignHorizontal.Left:
                            style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.UpperLeft);
                            break;
                        case TextAlignHorizontal.Right:
                            style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.UpperRight);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(horizontal), horizontal, null);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(vertical), vertical, null);
            }
        }
    }
}