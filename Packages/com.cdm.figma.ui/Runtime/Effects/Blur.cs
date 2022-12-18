using Cdm.Figma.UI.Styles;
using UnityEngine;

namespace Cdm.Figma.UI.Effects
{
    public abstract class Blur : EffectBehaviour
    {
        [SerializeField]
        private BlurType _type;

        public BlurType type
        {
            get => _type;
            set
            {
                _type = value;
                UpdateEffect();
            }
        }
        
        [SerializeField]
        private float _radius;
        
        public float radius
        {
            get => _radius;
            set
            {
                _radius = value;
                UpdateEffect();
            }
        }
    }
}