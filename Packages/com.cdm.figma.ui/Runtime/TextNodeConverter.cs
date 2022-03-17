using System.Linq;
using Cdm.Figma.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Cdm.Figma.UI
{
    public class TextNodeConverter : NodeConverter<TextNode>
    {
        public override NodeObject Convert(NodeObject parentObject, Node node, NodeConvertArgs args)
        {
            var textNode = (TextNode)node;
            var nodeObject = VectorNodeConverter.Convert(parentObject, textNode, args);

            var textComponent = nodeObject.gameObject.AddComponent<TextMeshProUGUI>();
            textComponent.text = textNode.characters;
            textComponent.characterSpacing = textNode.style.letterSpacing;
            textComponent.fontSize = textNode.style.fontSize;
            textComponent.fontWeight = (FontWeight)textNode.style.fontWeight;

            switch (@textNode.style.textDecoration)
            {
                case TextDecoration.Strikethrough:
                    textComponent.fontStyle |= FontStyles.Strikethrough;
                    break;
                case TextDecoration.Underline:
                    textComponent.fontStyle |= FontStyles.Underline;
                    break;
            }

            if (textNode.fills != null && textNode.fills.Any())
            {
                if (textNode.fills[0] is SolidPaint solidPaint)
                {
                    textComponent.color = solidPaint.color;
                }
            }

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

            return nodeObject;
        }
    }
}