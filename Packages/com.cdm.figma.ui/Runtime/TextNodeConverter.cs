using System;
using System.Collections.Generic;
using System.Linq;
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

            SetTextFont(textComponent, textNode, args);
            SetTextAlignment(textComponent, textNode);
            SetTextDecoration(textComponent, textNode);
            SetTextAutoResize(textComponent, textNode);
            SetFills(textComponent, textNode);
            SetLocalization(textComponent, nodeObject);
            
            return nodeObject;
        }

        private static void SetTextAlignment(TMP_Text textComponent, TextNode textNode)
        {
            switch (textNode.style.textAlignVertical)
            {
                case TextAlignVertical.Bottom:
                    textComponent.verticalAlignment = VerticalAlignmentOptions.Bottom;
                    break;
                case TextAlignVertical.Center:
                    textComponent.verticalAlignment = VerticalAlignmentOptions.Middle;
                    break;
                case TextAlignVertical.Top:
                    textComponent.verticalAlignment = VerticalAlignmentOptions.Top;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (textNode.style.textAlignHorizontal)
            {
                case TextAlignHorizontal.Center:
                    textComponent.horizontalAlignment = HorizontalAlignmentOptions.Center;
                    break;
                case TextAlignHorizontal.Justified:
                    textComponent.horizontalAlignment = HorizontalAlignmentOptions.Justified;
                    break;
                case TextAlignHorizontal.Left:
                    textComponent.horizontalAlignment = HorizontalAlignmentOptions.Left;
                    break;
                case TextAlignHorizontal.Right:
                    textComponent.horizontalAlignment = HorizontalAlignmentOptions.Right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SetTextAutoResize(TMP_Text textComponent, TextNode textNode)
        {
            switch (textNode.style.textAutoResize)
            {
                case TextAutoResize.None:
                    textComponent.autoSizeTextContainer = false;
                    break;
                case TextAutoResize.Height:
                    textComponent.autoSizeTextContainer = true;
                    textComponent.fontSizeMin = 1;
                    textComponent.fontSizeMax = 99;
                    break;
                case TextAutoResize.WidthAndHeight:
                    textComponent.autoSizeTextContainer = true;
                    textComponent.fontSizeMin = 1;
                    textComponent.fontSizeMax = 99;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private static void SetLocalization(TMP_Text textComponent, NodeObject nodeObject)
        {
            var localizationKey = nodeObject.localizationKey;
            if (!string.IsNullOrEmpty(localizationKey))
            {
                if (LocalizationHelper.TryGetTableAndEntryReference(
                        localizationKey, out var tableReference, out var tableEntryReference))
                {
                    var localizeStringEvent = nodeObject.gameObject.AddComponent<LocalizeStringEvent>();
                    localizeStringEvent.StringReference.SetReference(tableReference, tableEntryReference);
                    localizeStringEvent.OnUpdateString.AddListener(text => { textComponent.text = text; });
                    localizeStringEvent.RefreshString();
                }
                else
                {
                    Debug.LogWarning($"Localization key cannot be mapped: {localizationKey}");
                }
            }
        }
        
        
        private static void SetTextFont(TMP_Text textComponent, TextNode textNode, NodeConvertArgs args)
        {
            var fontName =
                FontSource.GetFontName(textNode.style.fontFamily, textNode.style.fontWeight, textNode.style.italic);

            if (args.file.TryGetFont(fontName, out var font))
            {
                textComponent.font = font;
            }
            else
            {
                Debug.LogWarning($"{fontName} could not be found.");
            }

            if (textNode.style.italic)
            {
                textComponent.fontStyle |= FontStyles.Italic;
            }
        }
        
        private static void SetTextDecoration(TMP_Text textComponent, TextNode textNode)
        {
            switch (textNode.style.textDecoration)
            {
                case TextDecoration.Strikethrough:
                    textComponent.fontStyle |= FontStyles.Strikethrough;
                    break;
                case TextDecoration.Underline:
                    textComponent.fontStyle |= FontStyles.Underline;
                    break;
            }
        }

        private static void SetFills(TMP_Text textComponent, TextNode textNode)
        {
            if (textNode.fills != null && textNode.fills.Any())
            {
                if (textNode.fills[0] is SolidPaint solidPaint)
                {
                    textComponent.color = solidPaint.color;
                }
            }
        }
    }
}