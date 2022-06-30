using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public class MaskStyle : Style
    {
        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            base.SetStyle(gameObject, args);

            GetOrAddComponent<Mask>(gameObject);
        }
    }
}