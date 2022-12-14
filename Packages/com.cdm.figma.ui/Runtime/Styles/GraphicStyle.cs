﻿using System;
using Cdm.Figma.UI.Styles.Properties;
using Cdm.Figma.UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public class GraphicStyle<T> : StyleWithSetter<T> where T : StyleSetterWithSelectorsBase
    {
        public StylePropertyColor color = new StylePropertyColor(UnityEngine.Color.white);
        public StylePropertyFloat fadeDuration = new StylePropertyFloat(0.1f);
        
        protected override void MergeTo(Style other, bool force)
        {
            if (other is GraphicStyle<T> otherStyle)
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
                    graphic.color = color.value;
                    
                    // TODO: If we enable cross fade, non-variant node colors dont get applied immediately.
                    //graphic.CrossFadeColor(args, color, fadeDuration);
                }
            }
        }
    }
    
    [Serializable]
    public class GraphicStyle : GraphicStyle<GraphicStyleSetter>
    {
    }
}