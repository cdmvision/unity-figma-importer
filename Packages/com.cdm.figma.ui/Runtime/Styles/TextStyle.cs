using System;
using Cdm.Figma.UI.Styles.Properties;
using TMPro;
using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    public class TextStyle : StyleWithSelectors<TextStylePropertyBlock>
    {
    }

    [Serializable]
    public class TextStylePropertyBlock : StylePropertyBlock
    {
        public StylePropertyFont font = new StylePropertyFont();
        public StylePropertyColor color = new StylePropertyColor(UnityEngine.Color.white);
        public StylePropertyFontStyle fontStyle = new StylePropertyFontStyle();
        public StylePropertyFontWeight fontWeight = new StylePropertyFontWeight(FontWeight.Regular);
        public StylePropertyMaterial fontMaterial = new StylePropertyMaterial();
        public StylePropertyFloat fontSize = new StylePropertyFloat();
        public StylePropertyBool autoSize = new StylePropertyBool();
        public StylePropertyTextAlignment alignment = new StylePropertyTextAlignment();
        public StylePropertyFloat characterSpacing = new StylePropertyFloat(0f);
        public StylePropertyFloat fadeDuration = new StylePropertyFloat(0.1f);

        public override void CopyTo(StylePropertyBlock other)
        {
            base.CopyTo(other);

            if (other is TextStylePropertyBlock otherStyle)
            {
                font.CopyTo(otherStyle.font);
                color.CopyTo(otherStyle.color);
                fontStyle.CopyTo(otherStyle.fontStyle);
                fontWeight.CopyTo(otherStyle.fontWeight);
                fontMaterial.CopyTo(otherStyle.fontMaterial);
                fontSize.CopyTo(otherStyle.fontSize);
                autoSize.CopyTo(otherStyle.autoSize);
                alignment.CopyTo(otherStyle.alignment);
                characterSpacing.CopyTo(otherStyle.characterSpacing);
            }
        }

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            base.SetStyle(gameObject, args);

            if (TryGetComponent<TMP_Text>(gameObject, out var text))
            {
                if (font.enabled)
                    text.font = font.value;

                if (fontStyle.enabled)
                    text.fontStyle = fontStyle.value;

                if (fontWeight.enabled)
                    text.fontWeight = fontWeight.value;

                if (fontSize.enabled)
                    text.fontSize = fontSize.value;

                if (autoSize.enabled)
                    text.enableAutoSizing = autoSize.value;

                if (alignment.enabled)
                    text.alignment = alignment.value;

                if (characterSpacing.enabled)
                    text.characterSpacing = characterSpacing.value;

                if (fontMaterial.enabled)
                    text.fontMaterial = fontMaterial.value;

                if (color.enabled)
                {
                    text.CrossFadeColor(args, color, fadeDuration);
                }
            }
        }
    }
}