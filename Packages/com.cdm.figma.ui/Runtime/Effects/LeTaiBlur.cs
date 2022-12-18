using Cdm.Figma.UI.Utils;

namespace Cdm.Figma.UI.Effects
{
    public class LeTaiBlur : Blur
    {
#if LETAI_TRANSLUCENTIMAGE
        private LeTai.Asset.TranslucentImage.TranslucentImage _image;

        protected override void Start()
        {
            _image = gameObject.GetOrAddComponent<LeTai.Asset.TranslucentImage.TranslucentImage>();

            UpdateEffect();
        }

        protected override void OnEnable()
        {
            if (_image != null)
            {
                _image.enabled = true;
            }
        }

        protected override void OnDisable()
        {
            if (_image != null)
            {
                _image.enabled = false;
            }
        }

        protected override void OnDestroy()
        {
            if (_image != null)
            {
                Destroy(_image);
            }
        }

        protected override void UpdateEffect()
        {
            // Translucent image has a global configuration and it needs to be configured manually.
        }
#else
        protected override void UpdateEffect() 
        {
            throw new NotImplementedException(
                "LeTai Translucent Image package must be installed and 'LETAI_TRANSLUCENTIMAGE' must be defined.");
        }
#endif
    }
}