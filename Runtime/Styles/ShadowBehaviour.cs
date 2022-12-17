using Cdm.Figma.UI.Utils;
using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    public class ShadowBehaviour : MonoBehaviour
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

#if LETAI_TRUESHADOW
        private LeTai.TrueShadow.TrueShadow _shadow;

        private void Start()
        {
            _shadow = gameObject.GetOrAddComponent<LeTai.TrueShadow.TrueShadow>();
            _shadow.UseCasterAlpha = true;
            _shadow.IgnoreCasterColor = true;

            UpdateEffect();
        }

        private void OnEnable()
        {
            if (_shadow != null)
            {
                _shadow.enabled = true;
            }
        }

        private void OnDisable()
        {
            if (_shadow != null)
            {
                _shadow.enabled = false;
            }
        }

        private void OnDestroy()
        {
            if (_shadow != null)
            {
                Destroy(_shadow);
            }
        }

        private void UpdateEffect()
        {
            if (_shadow != null)
            {
                _shadow.Inset = inner;
                _shadow.Color = color;
                _shadow.Size = radius;
                _shadow.Spread = spread;
                _shadow.OffsetDistance = _offset.magnitude;
                _shadow.OffsetAngle = Vector2.SignedAngle(Vector2.right, _offset); // TODO: test
                _shadow.BlendMode = Convert(blendMode);
            }
        }

        private static LeTai.TrueShadow.BlendMode Convert(BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.ColorDodge:
                    return LeTai.TrueShadow.BlendMode.Additive;
                case BlendMode.Multiply:
                    return LeTai.TrueShadow.BlendMode.Multiply;
                case BlendMode.Normal:
                    return LeTai.TrueShadow.BlendMode.Normal;
                case BlendMode.Screen:
                    return LeTai.TrueShadow.BlendMode.Screen;
                default:
                    return LeTai.TrueShadow.BlendMode.Normal;
            }
        }
#else
        private void UpdateEffect() {}
#endif
    }
}