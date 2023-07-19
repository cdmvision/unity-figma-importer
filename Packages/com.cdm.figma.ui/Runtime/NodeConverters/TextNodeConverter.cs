using System;
using System.Linq;
using Cdm.Figma.UI.Styles;
using Cdm.Figma.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class TextNodeConverter : VectorNodeConverter<TextNode, FigmaText>
    {
        protected override FigmaNode Convert(FigmaNode parentObject, TextNode textNode, NodeConvertArgs args)
        {
            var convertArgs = new VectorConvertArgs();
            convertArgs.generateSprite = false;

            var nodeObject = (FigmaText)base.Convert(parentObject, textNode, args, convertArgs);
            nodeObject.localizationKey = textNode.GetLocalizationKey();
            
            GenerateStyles(nodeObject, textNode, args);
            nodeObject.ApplyStyles();
            
            AddContentSizeFitterIfNeeded(nodeObject, textNode);

            return nodeObject;
        }

        private static void AddContentSizeFitterIfNeeded(FigmaNode nodeObject, TextNode textNode)
        {
            if (textNode.style.textAutoResize != TextAutoResize.None)
            {
                var fitter = nodeObject.gameObject.AddComponent<ContentSizeFitter>();
                if (textNode.style.textAutoResize == TextAutoResize.Height)
                {
                    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    fitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
                }
                else if (textNode.style.textAutoResize == TextAutoResize.WidthAndHeight)
                {
                    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                }
            }
        }

        private static void GenerateStyles(FigmaText nodeObject, TextNode textNode, NodeConvertArgs args)
        {
            var style = new TextStyle();
            style.enabled = true;

            SetTextValue(style, textNode, args);
            SetTextFont(style, textNode, args);
            SetTextAlignment(style, textNode);
            SetTextDecoration(style, textNode);
            SetTextAutoResize(style, textNode);
            SetFills(style, textNode);
            SetWordWrapping(style, textNode);
            
            nodeObject.styles.Add(style);
            
            // Disable text style property if localization style is added.
            if (TryConvertLocalization(nodeObject, args))
            {
                style.text.enabled = false;
            }
        }
        
        private static bool TryConvertLocalization(FigmaText node, NodeConvertArgs args)
        {
            if (args.importer.localizationConverter != null)
            {
                if (args.importer.localizationConverter.CanConvert(node))
                {
                    args.importer.localizationConverter.Convert(node);
                    return true;
                }
            }

            return false;
        }

        private static void SetWordWrapping(TextStyle style, TextNode textNode)
        {
            if (textNode.style.textAutoResize == TextAutoResize.None)
            {
                style.wordWrapping.enabled = true;
                style.wordWrapping.value = false;
            }
        }

        private static void SetTextValue(TextStyle style, TextNode textNode, NodeConvertArgs args)
        {
            var textValue = textNode.characters;
            
            var propertyReference = textNode.componentPropertyReferences?.characters;
            if (!string.IsNullOrEmpty(propertyReference))
            {
                if (args.textPropertyAssignments.TryGetValue(propertyReference, out var characters))
                {
                    textValue = characters;
                }
            }

            style.text.enabled = true;
            style.text.value = textValue;
        }

        private static void SetTextFont(TextStyle style, TextNode textNode, NodeConvertArgs args)
        {
            var fontName = FontHelpers.GetFontDescriptor(
                textNode.style.fontFamily, textNode.style.fontWeight, textNode.style.italic);

            if (args.importer.TryGetFont(fontName, out var font))
            {
                style.font.enabled = true;
                style.font.value = font;

                args.importer.dependencyAssets.Add(fontName, font);
            }
            else
            {
                style.font.enabled = false;
                //Debug.LogWarning($"{fontName} could not be found.");
            }

            if (textNode.style.italic)
            {
                style.fontStyle.enabled = true;
                style.fontStyle.value |= FontStyles.Italic;
            }

            style.characterSpacing.enabled = true;
            style.characterSpacing.value = textNode.style.letterSpacing;

            style.fontSize.enabled = true;
            style.fontSize.value = textNode.style.fontSize;

            style.fontWeight.enabled = true;
            style.fontWeight.value = (FontWeight)textNode.style.fontWeight;
        }

        private static void SetTextAlignment(TextStyle style, TextNode textNode)
        {
            style.verticalAlignment.enabled = true;
            style.horizontalAlignment.enabled = true;

            switch (textNode.style.textAlignVertical)
            {
                case TextAlignVertical.Bottom:
                    style.verticalAlignment.value = VerticalAlignmentOptions.Bottom;
                    break;
                case TextAlignVertical.Center:
                    style.verticalAlignment.value = VerticalAlignmentOptions.Middle;
                    break;
                case TextAlignVertical.Top:
                    style.verticalAlignment.value = VerticalAlignmentOptions.Top;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (textNode.style.textAlignHorizontal)
            {
                case TextAlignHorizontal.Center:
                    style.horizontalAlignment.value = HorizontalAlignmentOptions.Center;
                    break;
                case TextAlignHorizontal.Justified:
                    style.horizontalAlignment.value = HorizontalAlignmentOptions.Justified;
                    break;
                case TextAlignHorizontal.Left:
                    style.horizontalAlignment.value = HorizontalAlignmentOptions.Left;
                    break;
                case TextAlignHorizontal.Right:
                    style.horizontalAlignment.value = HorizontalAlignmentOptions.Right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SetTextAutoResize(TextStyle style, TextNode textNode)
        {
            style.autoSizeTextContainer.enabled = true;

            switch (textNode.style.textAutoResize)
            {
                case TextAutoResize.None:
                    style.autoSizeTextContainer.value = false;
                    style.fontSizeMin.enabled = false;
                    style.fontSizeMax.enabled = false;
                    break;
                case TextAutoResize.Height:
                case TextAutoResize.WidthAndHeight:
                    style.autoSizeTextContainer.value = true;
                    style.fontSizeMin.enabled = true;
                    style.fontSizeMin.value = 1;
                    style.fontSizeMax.enabled = true;
                    style.fontSizeMax.value = 99;
                    break;
                case TextAutoResize.Truncate:
                    style.enableTruncate.enabled = true;
                    style.enableTruncate.value = TextOverflowModes.Ellipsis;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SetTextDecoration(TextStyle style, TextNode textNode)
        {
            switch (textNode.style.textDecoration)
            {
                case TextDecoration.None:
                    style.fontStyle.enabled = false;
                    break;
                case TextDecoration.Strikethrough:
                    style.fontStyle.enabled = true;
                    style.fontStyle.value |= FontStyles.Strikethrough;
                    break;
                case TextDecoration.Underline:
                    style.fontStyle.enabled = true;
                    style.fontStyle.value |= FontStyles.Underline;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SetFills(TextStyle style, TextNode textNode)
        {
            style.color.enabled = false;

            if (textNode.fills != null && textNode.fills.Any())
            {
                if (textNode.fills[0] is SolidPaint solidPaint)
                {
                    style.color.enabled = true;
                    style.color.value = solidPaint.color;
                }
            }
        }
    }
}