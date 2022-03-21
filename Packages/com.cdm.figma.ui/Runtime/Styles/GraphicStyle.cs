﻿using System;
using Cdm.Figma.UI.Styles.Properties;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Styles
{
    public class GraphicStyle : StyleWithSelectors<GraphicStylePropertyBlock>
    {
    }
    
    [Serializable]
    public class GraphicStylePropertyBlock : StylePropertyBlock
    {
        public StylePropertyColor color = new StylePropertyColor(UnityEngine.Color.white);
        public StylePropertyFloat fadeDuration = new StylePropertyFloat(0.1f);
        
        public override void CopyTo(StylePropertyBlock other)
        {
            base.CopyTo(other);
            
            if (other is GraphicStylePropertyBlock otherStyle)
            {
                color.CopyTo(otherStyle.color);
                fadeDuration.CopyTo(otherStyle.fadeDuration);
            }
        }

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            base.SetStyle(gameObject, args);
            
            if (TryGetComponent<Graphic>(gameObject, out var graphic))
            {
                if (color.enabled)
                {
                    graphic.CrossFadeColor(args, color, fadeDuration);
                }
            }
        }
    }

    public static class GraphicExtensions
    {
        public static void CrossFadeColor(this Graphic graphic, StyleArgs args, 
            StylePropertyColor color, StylePropertyFloat fadeDuration)
        {
            var duration = !fadeDuration.enabled || args.instant ? 0f : fadeDuration.value;
            graphic.CrossFadeColor(color.value, duration, false, true);    
        }
    }
}