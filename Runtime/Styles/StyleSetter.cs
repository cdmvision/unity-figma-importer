using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    public abstract class StyleSetter : MonoBehaviour
    {
        protected string currentSelector { get; private set; }
        
        public virtual void CopyTo(StyleSetter other)
        {
        }

        public void Apply(string selector, bool instant = false)
        {
            if (currentSelector != selector)
            {
                currentSelector = selector;
                Apply(new StyleArgs(currentSelector, instant));
            }
        }
        
        protected abstract void Apply(StyleArgs args);
    }
    
    public readonly struct StyleArgs
    {
        public string selector { get; }
        public bool instant { get; }

        public StyleArgs(string selector)
        {
            this.selector = selector;
            this.instant = false;
        }

        public StyleArgs(string selector, bool instant)
        {
            this.selector = selector;
            this.instant = instant;
        }
    }
}