using Cdm.Figma.UI.Utils;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class LeTaiShadowEffectBehaviour : ShadowEffectBehaviour
    {
#if LETAI_TRUESHADOW
        private LeTai.TrueShadow.TrueShadow _shadow;

        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                _shadow = gameObject.AddComponent<LeTai.TrueShadow.TrueShadow>();
                _shadow.UseCasterAlpha = true;
                _shadow.IgnoreCasterColor = true;
                UpdateEffect();
            }
        }
        
        protected override void OnEnable()
        {
            if (_shadow != null)
            {
                _shadow.enabled = true;
            }
        }

        protected override void OnDisable()
        {
            if (_shadow != null)
            {
                _shadow.enabled = false;
            }
        }

        protected override void OnDestroy()
        {
            if (_shadow != null)
            {
                Destroy(_shadow);
            }
        }

        protected override void UpdateEffect()
        {
            if (_shadow != null)
            {
                _shadow.Inset = inner;
                _shadow.Color = color;
                _shadow.Size = radius;
                _shadow.Spread = spread;
                _shadow.OffsetDistance = offset.magnitude;
                _shadow.OffsetAngle = Vector2.SignedAngle(Vector2.right, offset); // TODO: test
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
        protected override void Awake()
        {
            base.Awake();

            Debug.LogError("LeTai True Shadow package must be installed and 'LETAI_TRUESHADOW' must be defined.", this);
            enabled = false;
        }
        
        protected override void UpdateEffect()
        {
        }
#endif
    }
}