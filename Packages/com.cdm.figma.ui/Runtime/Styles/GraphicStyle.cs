using System;
using Cdm.Figma.UI.Styles.Properties;
using Cdm.Figma.UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public class GraphicStyle : Style
    {
        public StylePropertyColor color = new StylePropertyColor(UnityEngine.Color.white);
        public StylePropertyFloat fadeDuration = new StylePropertyFloat(0.1f);

        public override void SetStyleAsSelector(GameObject gameObject, StyleArgs args)
        {
        }

        protected override void MergeTo(Style other, bool force)
        {
            if (other is GraphicStyle otherStyle)
            {
                OverwriteProperty(color, otherStyle.color, force);
                OverwriteProperty(fadeDuration, otherStyle.fadeDuration, force);
            }
        }

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            var graphic = gameObject.GetOrAddComponent<Graphic>();
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