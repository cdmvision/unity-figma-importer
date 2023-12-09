using System;
using Cdm.Figma.UI.Styles.Properties;
using Cdm.Figma.UI.Utils;
using TMPro;
using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public class TextStyle : GraphicStyle<TextStyleSetter>
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
        public StylePropertyBool wordWrapping = new StylePropertyBool();
        public StylePropertyBool autoSizeTextContainer = new StylePropertyBool();
        public StylePropertyFloat characterSpacing = new StylePropertyFloat(0f);
        public StylePropertyTextOverflowMode overflowMode = new StylePropertyTextOverflowMode(TextOverflowModes.Overflow);

        public StylePropertyHorizontalAlignmentOptions horizontalAlignment =
            new StylePropertyHorizontalAlignmentOptions();

        public StylePropertyVerticalAlignmentOptions verticalAlignment =
            new StylePropertyVerticalAlignmentOptions();

        protected override void MergeTo(Style other, bool force)
        {
            base.MergeTo(other, force);

            if (other is TextStyle otherStyle)
            {
                OverwriteProperty(text, otherStyle.text, force);
                OverwriteProperty(font, otherStyle.font, force);
                OverwriteProperty(fontStyle, otherStyle.fontStyle, force);
                OverwriteProperty(fontWeight, otherStyle.fontWeight, force);
                OverwriteProperty(fontMaterial, otherStyle.fontMaterial, force);
                OverwriteProperty(fontSize, otherStyle.fontSize, force);
                OverwriteProperty(fontSizeMin, otherStyle.fontSizeMin, force);
                OverwriteProperty(fontSizeMax, otherStyle.fontSizeMax, force);
                OverwriteProperty(enableAutoSizing, otherStyle.enableAutoSizing, force);
                OverwriteProperty(overflowMode, otherStyle.overflowMode, force);
                OverwriteProperty(wordWrapping, otherStyle.wordWrapping, force);
                OverwriteProperty(autoSizeTextContainer, otherStyle.autoSizeTextContainer, force);
                OverwriteProperty(horizontalAlignment, otherStyle.horizontalAlignment, force);
                OverwriteProperty(verticalAlignment, otherStyle.verticalAlignment, force);
                OverwriteProperty(characterSpacing, otherStyle.characterSpacing, force);
            }
        }

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            var textComponent = gameObject.GetOrAddComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                base.SetStyle(gameObject, args);

                textComponent.raycastTarget = false;

                // Set text value even if it is not enabled for setting as default text value when importing.
                if (text.enabled || !Application.isPlaying)
                    textComponent.text = text.value;

                if (font.enabled)
                    textComponent.font = font.value;
                
                if (fontStyle.enabled)
                    textComponent.fontStyle = fontStyle.value;

                if (fontWeight.enabled)
                    textComponent.fontWeight = (TMPro.FontWeight)fontWeight.value;

                if (fontSize.enabled)
                    textComponent.fontSize = fontSize.value;

                if (fontSizeMin.enabled)
                    textComponent.fontSizeMin = fontSizeMin.value;

                if (fontSizeMax.enabled)
                    textComponent.fontSizeMax = fontSizeMax.value;

                if (enableAutoSizing.enabled)
                    textComponent.enableAutoSizing = enableAutoSizing.value;
                
                if (overflowMode.enabled)
                    textComponent.overflowMode = overflowMode.value;
                 
                if (wordWrapping.enabled)
                    textComponent.enableWordWrapping = wordWrapping.value;

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
            }
        }
    }
}