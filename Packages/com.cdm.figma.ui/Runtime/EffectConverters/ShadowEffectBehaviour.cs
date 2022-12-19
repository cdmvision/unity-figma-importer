using UnityEngine;

namespace Cdm.Figma.UI
{
    public abstract class ShadowEffectBehaviour : EffectBehaviour
    {
        [SerializeField]
        private bool _inner;

        public bool inner
        {
            get => _inner;
            set
            {
                _inner = value;
                UpdateEffect();
            }
        }

        [SerializeField]
        private UnityEngine.Color _color;

        public UnityEngine.Color color
        {
            get => _color;
            set
            {
                _color = value;
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

        [SerializeField]
        private float _spread;

        public float spread
        {
            get => _spread;
            set
            {
                _spread = value;
                UpdateEffect();
            }
        }

        [SerializeField]
        private Vector2 _offset;

        public Vector2 offset
        {
            get => _offset;
            set
            {
                _offset = value;
                UpdateEffect();
            }
        }

        [SerializeField]
        private BlendMode _blendMode;

        public BlendMode blendMode
        {
            get => _blendMode;
            set
            {
                _blendMode = value;
                UpdateEffect();
            }
        }
    }
}