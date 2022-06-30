using System;
using System.Collections.Generic;
using System.Linq;
using Cdm.Figma.UI.Styles;
using Cdm.Figma.UI.Styles.Properties;
using Cdm.Figma.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Cdm.Figma.UI
{
    public class TextNodeConverter : VectorNodeConverter<TextNode>
    {
        protected override NodeObject Convert(NodeObject parentObject, TextNode textNode, NodeConvertArgs args)
        {
            var convertArgs = new VectorConvertArgs();
            convertArgs.generateSprite = false;

            var nodeObject = base.Convert(parentObject, textNode, args, convertArgs);

            var textComponent = nodeObject.gameObject.AddComponent<TextMeshProUGUI>();
            textComponent.text = textNode.characters;
            textComponent.characterSpacing = textNode.style.letterSpacing;
            textComponent.fontSize = textNode.style.fontSize;
            textComponent.fontWeight = (FontWeight)textNode.style.fontWeight;

            //SetTextFont(textComponent, textNode, args);
            //SetTextAlignment(textComponent, textNode);
            //SetTextDecoration(textComponent, textNode);
            //SetTextAutoResize(textComponent, textNode);
            //SetFills(textComponent, textNode);
            //SetLocalization(textComponent, nodeObject);

            var style = GenerateStyle(nodeObject, textNode, args);
            style.SetStyle(nodeObject.gameObject, new StyleArgs(Selector.Normal, true));
            
            return nodeObject;
        }

        public Styles.Style GenerateStyle(NodeObject nodeObject, TextNode textNode, NodeConvertArgs args)
        {
            var style = new TextStyle();

            SetTextFont(style, textNode, args);
            SetTextAlignment(style, textNode);
            SetTextDecoration(style, textNode);
            SetTextAutoResize(style, textNode);
            SetFills(style, textNode);
            SetLocalization(style, nodeObject);
            
            return style;
        }

        private static void SetTextFont(TextStyle style, TextNode textNode, NodeConvertArgs args)
        {
            var fontName =
                FontSource.GetFontName(textNode.style.fontFamily, textNode.style.fontWeight, textNode.style.italic);

            if (args.file.TryGetFont(fontName, out var font))
            {
                style.font.enabled = true;
                style.font.value = font;
            }
            else
            {
                style.font.enabled = false;
                Debug.LogWarning($"{fontName} could not be found.");
            }

            if (textNode.style.italic)
            {
                style.fontStyle.enabled = true;
                style.fontStyle.value |= FontStyles.Italic;
            }
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
        
        private static void SetLocalization(TextStyle style, NodeObject nodeObject)
        {
            style.localizedString.enabled = false;
            
            var localizationKey = nodeObject.localizationKey;
            if (!string.IsNullOrEmpty(localizationKey))
            {
                if (LocalizationHelper.TryGetTableAndEntryReference(
                        localizationKey, out var tableReference, out var tableEntryReference))
                {
                    style.localizedString.enabled = true;
                    style.localizedString.value.SetReference(tableReference, tableEntryReference);
                }
                else
                {
                    Debug.LogWarning($"Localization key cannot be mapped: {localizationKey}");
                }
            }
        }
    }
}