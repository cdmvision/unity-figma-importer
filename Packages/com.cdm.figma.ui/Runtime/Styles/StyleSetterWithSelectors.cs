using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    public abstract class StyleSetterWithSelectors<T> : StyleSetter where T : Style, new()
    {
        [SerializeField]
        private List<T> _styles = new List<T>();
        
        public override void CopyTo(StyleSetter other)
        {
            base.CopyTo(other);

            if (other is StyleSetterWithSelectors<T> o)
            {
                o._styles = new List<T>();

                foreach (var style in _styles)
                {
                    var otherStyle = new T();
                    style.CopyTo(otherStyle);
                    
                    o._styles.Add(otherStyle);
                }
            }
        }
        
        protected override void Apply(StyleArgs args)
        {
            var style = _styles.FirstOrDefault(s => s.selector == args.selector);
            if (style != null)
            {
                SetStyleOrNormal(args, style);
            }
            else
            {
                Debug.LogWarning($"Style with selector could not found: {args.selector}");
            }
        }

        private void SetStyleOrNormal(StyleArgs args, T styleBlock)
        {
            if (!styleBlock.SetStyleIfEnabled(gameObject, args))
            {
                var style = _styles.FirstOrDefault();
                style?.SetStyleIfEnabled(gameObject, args);
            }
        }
    }
}