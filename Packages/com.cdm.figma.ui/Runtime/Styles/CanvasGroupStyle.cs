﻿using System;
using Cdm.Figma.UI.Styles.Properties;
using Cdm.Figma.UI.Utils;
using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public class CanvasGroupStyle : StyleWithSetter<CanvasGroupStyleSetter>
    {
        public StylePropertyFloat alpha = new StylePropertyFloat(1f);

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            var canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                if (alpha.enabled)
                    canvasGroup.alpha = alpha.value;
            }
        }
        
        protected override void MergeTo(Style other, bool force)
        {
            if (other is CanvasGroupStyle otherStyle)
            {
                OverwriteProperty(alpha, otherStyle.alpha, force);
            }
        }
    }
}