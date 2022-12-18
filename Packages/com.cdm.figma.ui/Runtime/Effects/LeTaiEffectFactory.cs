using Cdm.Figma.UI.Styles;
using UnityEngine;

namespace Cdm.Figma.UI.Effects
{
    public class LeTaiEffectFactory : IEffectFactory
    {
        public bool Add(GameObject gameObject, Styles.Style style)
        {
            switch (style)
            {
                case BlurStyle:
                    
                    return false;
                case ShadowStyle:
                    gameObject.AddComponent<LeTaiShadow>();
                    return true;
            }
            
            return true;
        }
    }
}