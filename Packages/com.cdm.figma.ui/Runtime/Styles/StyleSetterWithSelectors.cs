using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    public abstract class StyleSetterWithSelectorsBase : StyleSetter
    {
        public abstract IEnumerable<Style> GetStyles();
        public abstract Style GetStyle(Predicate<Style> predicate);
        public abstract void AddStyle(Style style);
        public abstract int RemoveStyles(Predicate<Style> predicate);
    }
    
    public abstract class StyleSetterWithSelectors<T> : StyleSetterWithSelectorsBase where T : Style, new()
    {
        [SerializeField]
        private List<T> _styles = new List<T>();

        public IList<T> styles => _styles;

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

        public override IEnumerable<Style> GetStyles()
        {
            return styles;
        }

        public override Style GetStyle(Predicate<Style> predicate)
        {
            return styles.FirstOrDefault(x => predicate(x));
        }

        public override void AddStyle(Style style)
        {
            if (style is not T s)
                throw new ArgumentException(
                    $"Invalid style type got: {style.GetType()}, expected: {typeof(T)}", nameof(style));
            
            styles.Add(s);
        }

        public override int RemoveStyles(Predicate<Style> predicate)
        {
            return _styles.RemoveAll(predicate);
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
                if (Application.isPlaying)
                {
                    Debug.LogWarning($"Style with selector could not found: {args.selector}", this);    
                }
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