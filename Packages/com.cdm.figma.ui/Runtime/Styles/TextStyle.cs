using System;
using Cdm.Figma.UI.Styles.Properties;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public class TextStyle : GraphicStyle
    {
        public StylePropertyString text = new StylePropertyString();
        public StylePropertyFont font = new StylePropertyFont();
        public StylePropertyFontStyle fontStyle = new StylePropertyFontStyle();
        public StylePropertyFontWeight fontWeight = new StylePropertyFontWeight(FontWeight.Regular);
        public StylePropertyMaterial fontMaterial = new StylePropertyMaterial();
        public StylePropertyFloat fontSize = new StylePropertyFloat();
        public StylePropertyFloat fontSizeMin = new StylePropertyFloat();
        public StylePropertyFloat fontSizeMax = new StylePropertyFloat();
        public StylePropertyBool enableAutoSizing = new StylePropertyBool();
        public StylePropertyBool autoSizeTextContainer = new StylePropertyBool();
        public StylePropertyHorizontalAlignmentOptions horizontalAlignment =
            new StylePropertyHorizontalAlignmentOptions();
        public StylePropertyVerticalAlignmentOptions verticalAlignment = 
            new StylePropertyVerticalAlignmentOptions();
        public StylePropertyFloat characterSpacing = new StylePropertyFloat(0f);
        public StylePropertyLocalizedString localizedString = new StylePropertyLocalizedString();

        public override void CopyTo(Style other)
        {
            base.CopyTo(other);

            if (other is TextStyle otherStyle)
            {
                text.CopyTo(otherStyle.text);
                font.CopyTo(otherStyle.font);
                color.CopyTo(otherStyle.color);
                fontStyle.CopyTo(otherStyle.fontStyle);
                fontWeight.CopyTo(otherStyle.fontWeight);
                fontMaterial.CopyTo(otherStyle.fontMaterial);
                fontSize.CopyTo(otherStyle.fontSize);
                fontSizeMin.CopyTo(otherStyle.fontSizeMin);
                fontSizeMax.CopyTo(otherStyle.fontSizeMax);
                enableAutoSizing.CopyTo(otherStyle.enableAutoSizing);
                autoSizeTextContainer.CopyTo(otherStyle.autoSizeTextContainer);
                horizontalAlignment.CopyTo(otherStyle.horizontalAlignment);
                verticalAlignment.CopyTo(otherStyle.verticalAlignment);
                characterSpacing.CopyTo(otherStyle.characterSpacing);
                localizedString.CopyTo(otherStyle.localizedString);
            }
        }

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            var textComponent = GetOrAddComponent<TextMeshProUGUI>(gameObject);
            if (textComponent != null)
            {
                base.SetStyle(gameObject, args);

                if (textComponent.enabled)
                    textComponent.text = text.value;
                
                if (font.enabled)
                    textComponent.font = font.value;

                if (fontStyle.enabled)
                    textComponent.fontStyle = fontStyle.value;

                if (fontWeight.enabled)
                    textComponent.fontWeight = fontWeight.value;

                if (fontSize.enabled)
                    textComponent.fontSize = fontSize.value;

                if (fontSizeMin.enabled)
                    textComponent.fontSizeMin = fontSizeMin.value;

                if (fontSizeMax.enabled)
                    textComponent.fontSizeMax = fontSizeMax.value;

                if (enableAutoSizing.enabled)
                    textComponent.enableAutoSizing = enableAutoSizing.value;

                if (autoSizeTextContainer.enabled)
                    textComponent.autoSizeTextContainer = autoSizeTextContainer.value;

                if (horizontalAlignment.enabled)
                    textComponent.horizontalAlignment = horizontalAlignment.value;

                if (verticalAlignment.enabled)
                    textComponent.verticalAlignment = verticalAlignment.value;

                if (characterSpacing.enabled)
                    textComponent.characterSpacing = characterSpacing.value;

                if (fontMaterial.enabled)
                    textComponent.fontMaterial = fontMaterial.value;

                if (localizedString.enabled)
                {
                    var stringEvent = GetOrAddComponent<LocalizeStringEvent>(textComponent.gameObject);
                    stringEvent.StringReference = localizedString.value;
                    stringEvent.OnUpdateString.AddListener(str => { textComponent.text = str; });
                    stringEvent.RefreshString();
                }
            }
        }
    }
}