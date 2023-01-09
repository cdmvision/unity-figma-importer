using System;
using UnityEngine;

namespace Cdm.Figma.UI.Styles.Properties
{
    [Serializable]
    public class StylePropertySprite : StyleProperty<Sprite>
    {
        public StylePropertySprite()
        {
        }

        public StylePropertySprite(Sprite defaultValue) : base(defaultValue)
        {
        }
    }
}