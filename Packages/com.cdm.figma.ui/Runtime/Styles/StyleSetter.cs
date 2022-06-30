using UnityEngine;

namespace Cdm.Figma.UI.Styles
{
    /// <summary>
    /// An enumeration of selected states of objects
    /// </summary>
    public enum Selector
    {
        /// <summary>
        /// The UI object can be selected.
        /// </summary>
        Normal,

        /// <summary>
        /// The UI object is highlighted.
        /// </summary>
        Highlighted,

        /// <summary>
        /// The UI object is pressed.
        /// </summary>
        Pressed,
        
        /// <summary>
        /// The UI object is selected.
        /// </summary>
        Selected,

        /// <summary>
        /// The UI object cannot be selected.
        /// </summary>
        Disabled,
    }
    
    public abstract class StyleSetter : MonoBehaviour
    {
        protected Selector currentSelector { get; private set; } = Selector.Normal;
        
        private bool _interactable = true;

        public bool interactable
        {
            get => _interactable;
            set
            {
                if (_interactable != value)
                {
                    _interactable = value;
                
                    Apply(_interactable ? Selector.Normal : Selector.Disabled, true);
                }
            }
        }

        protected virtual void Start()
        {
            Apply(Selector.Normal, true, true);
        }

        public virtual void CopyTo(StyleSetter other)
        {
            other.interactable = interactable;
        }

        public void Apply(Selector selector, bool forceUpdate = false)
        {
            Apply(selector, false, forceUpdate);
        }
        
        protected abstract void Apply(StyleArgs args);

        private void Apply(Selector selector, bool instant, bool forceUpdate)
        {
            if (!_interactable)
                selector = Selector.Disabled;

            if (forceUpdate || currentSelector != selector)
            {
                currentSelector = selector;
                Apply(new StyleArgs(currentSelector, instant));
            }
        }
    }
    
    public readonly struct StyleArgs
    {
        public Selector selector { get; }
        public bool instant { get; }

        public StyleArgs(Selector selector)
        {
            this.selector = selector;
            this.instant = false;
        }

        public StyleArgs(Selector selector, bool instant)
        {
            this.selector = selector;
            this.instant = instant;
        }
    }
}