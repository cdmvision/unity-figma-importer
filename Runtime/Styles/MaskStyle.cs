using System;
using Cdm.Figma.UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public class MaskStyle : Style
    {
        public override void SetStyleAsSelector(GameObject gameObject, StyleArgs args)
        {
        }

        protected override void MergeTo(Style other, bool force)
        {
        }

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            var mask = gameObject.GetOrAddComponent<Mask>();
            mask.showMaskGraphic = true;
        }
    }
}