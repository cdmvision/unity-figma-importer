using System;
using System.Linq;
using Cdm.Figma.UI.Styles;
using Cdm.Figma.UI.Utils;
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
                var contentSizeFitter = nodeObject.gameObject.GetOrAddComponent<ContentSizeFitter>();
                
                if (textNode.style.textAutoResize == TextAutoResize.Height)
                {
                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
                }
                else if (textNode.style.textAutoResize == TextAutoResize.WidthAndHeight)
                {
                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
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
            SetTextCase(style, textNode);
            SetTextAutoResize(style, textNode);
            SetTextTruncation(style, textNode);
            SetFills(style, textNode);

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
                    Debug.LogWarning(
                        $"{nameof(TextAlignVertical)} value is not handled: '{textNode.style.textAlignVertical}'");
                    break;
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
                    Debug.LogWarning(
                        $"{nameof(TextAlignHorizontal)} value is not handled: '{textNode.style.textAlignHorizontal}'");
                    break;
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
#pragma warning disable CS0618
                case TextAutoResize.Truncate:
                    // It is deprecated.
#pragma warning restore CS0618
                    break;
                default:
                    Debug.LogWarning(
                        $"{nameof(TextAutoResize)} value is not handled: '{textNode.style.textAutoResize}'");
                    break;
            }
        }

        public static void SetTextTruncation(TextStyle style, TextNode textNode)
        {
            var textTruncation = textNode.style.textTruncation ?? TextTruncation.Disabled;
            
            if (textNode.style.textAutoResize == TextAutoResize.WidthAndHeight)
            {
                //if text autosize is set to auto width, there is no need to enable word wrapping
                style.wordWrapping.enabled = true;
                style.wordWrapping.value = false;
            }
            
            switch (textTruncation)
            {
                case TextTruncation.Disabled:
                    style.overflowMode.enabled = true;
                    style.overflowMode.value = TextOverflowModes.Overflow;
                    break;
                case TextTruncation.Ending:
                    style.overflowMode.enabled = true;
                    style.overflowMode.value = TextOverflowModes.Ellipsis;
                    break;
                default:
                    Debug.LogWarning($"{nameof(TextTruncation)} value is not handled: '{textTruncation}'");
                    break;
            }
        }

        private static void SetTextDecoration(TextStyle style, TextNode textNode)
        {
            switch (textNode.style.textDecoration)
            {
                case TextDecoration.None:
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
                    Debug.LogWarning(
                        $"{nameof(TextDecoration)} value is not handled: '{textNode.style.textDecoration}'");
                    break;
            }

            style.fontStyle.enabled = true;
        }

        private static void SetTextCase(TextStyle style, TextNode textNode)
        {
            switch (textNode.style.textCase)
            {
                case TextCase.Original:
                    break;
                case TextCase.Upper:
                    style.fontStyle.enabled = true;
                    style.fontStyle.value |= FontStyles.UpperCase;
                    break;
                case TextCase.Lower:
                    style.fontStyle.enabled = true;
                    style.fontStyle.value |= FontStyles.LowerCase;
                    break;
                case TextCase.Title:
                    break;
                case TextCase.SmallCaps:
                    style.fontStyle.enabled = true;
                    style.fontStyle.value |= FontStyles.SmallCaps;
                    break;
                case TextCase.SmallCapsForced:
                    style.fontStyle.enabled = true;
                    style.fontStyle.value |= FontStyles.SmallCaps;
                    break;
                default:
                    Debug.LogWarning($"{nameof(TextCase)} value is not handled: '{textNode.style.textCase}'");
                    break;
            }
            
            style.fontStyle.enabled = true;
        }

        private static void SetFills(TextStyle style, TextNode textNode)
        {
            style.color.enabled = false;

            if (textNode.fills != null && textNode.fills.Any())
            {
                if (textNode.fills[0] is SolidPaint solidPaint)
                {
                    var color = (UnityEngine.Color)solidPaint.color;
                    color.a = solidPaint.opacity;
                    
                    style.color.enabled = true;
                    style.color.value = color;
                }
            }
        }
    }
}