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
            var text = GetOrAddComponent<TextMeshProUGUI>(gameObject);
            if (text != null)
            {
                base.SetStyle(gameObject, args);
                
                if (font.enabled)
                    text.font = font.value;

                if (fontStyle.enabled)
                    text.fontStyle = fontStyle.value;

                if (fontWeight.enabled)
                    text.fontWeight = fontWeight.value;

                if (fontSize.enabled)
                    text.fontSize = fontSize.value;

                if (fontSizeMin.enabled)
                    text.fontSizeMin = fontSizeMin.value;

                if (fontSizeMax.enabled)
                    text.fontSizeMax = fontSizeMax.value;

                if (enableAutoSizing.enabled)
                    text.enableAutoSizing = enableAutoSizing.value;

                if (autoSizeTextContainer.enabled)
                    text.autoSizeTextContainer = autoSizeTextContainer.value;

                if (horizontalAlignment.enabled)
                    text.horizontalAlignment = horizontalAlignment.value;

                if (verticalAlignment.enabled)
                    text.verticalAlignment = verticalAlignment.value;

                if (characterSpacing.enabled)
                    text.characterSpacing = characterSpacing.value;

                if (fontMaterial.enabled)
                    text.fontMaterial = fontMaterial.value;

                if (localizedString.enabled)
                {
                    var stringEvent = GetOrAddComponent<LocalizeStringEvent>(text.gameObject);
                    stringEvent.StringReference = localizedString.value;
                    stringEvent.OnUpdateString.AddListener(str => { text.text = str; });
                    stringEvent.RefreshString();
                }
            }
        }
    }
}