using System;
using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    [Serializable]
    public enum BlurType
    {
        Layer,
        Background
    }
    
    public class BlurBehaviour : MonoBehaviour
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

        private void UpdateEffect()
        {
            // TODO: implement
        }
    }
}