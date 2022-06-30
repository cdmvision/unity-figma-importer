using System;
using Cdm.Figma.UI.Styles.Properties;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public class GraphicStyle : Style
    {
        public StylePropertyColor color = new StylePropertyColor(UnityEngine.Color.white);
        public StylePropertyFloat fadeDuration = new StylePropertyFloat(0.1f);

        public override void CopyTo(Style other)
        {
            base.CopyTo(other);

            if (other is GraphicStyle otherStyle)
            {
                color.CopyTo(otherStyle.color);
                fadeDuration.CopyTo(otherStyle.fadeDuration);
            }
        }

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            base.SetStyle(gameObject, args);

            var graphic = GetOrAddComponent<Graphic>(gameObject);
            if (graphic != null)
            {
                if (color.enabled)
                {
                    graphic.CrossFadeColor(args, color, fadeDuration);
                }
            }
        }
    }
}