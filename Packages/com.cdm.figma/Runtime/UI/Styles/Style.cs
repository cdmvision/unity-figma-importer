using UnityEngine;
using UnityEngine.EventSystems;

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
    
    public abstract class Style : MonoBehaviour
    {
        protected Selector currentSelector { get; private set; } = Selector.Normal;
        
        private bool _interactable = true;

        public virtual void CopyTo(Style other)
        {
        }

        public abstract void Apply(StyleArgs args);

        public virtual void OnInteractableChanged(bool value)
        {
            if (_interactable != value)
            {
                _interactable = value;
                UpdateCurrentSelector(_interactable ? Selector.Normal : Selector.Disabled, true);
            }
        }
        
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            UpdateCurrentSelector(Selector.Pressed);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            UpdateCurrentSelector(Selector.Highlighted);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            UpdateCurrentSelector(Selector.Highlighted);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            UpdateCurrentSelector(Selector.Normal);
        }
        
        public virtual void OnSelect(BaseEventData eventData)
        {
            UpdateCurrentSelector(Selector.Selected);
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            UpdateCurrentSelector(Selector.Normal);
        }
        
        protected virtual void UpdateCurrentSelector(Selector current, bool forceUpdate = false)
        {
            if (!_interactable)
                current = Selector.Disabled;

            if (forceUpdate || currentSelector != current)
            {
                currentSelector = current;
                Apply(new StyleArgs(currentSelector, false));
            }
        }
    }
    
    public readonly struct StyleArgs
    {
        public Selector selector { get; }
        public bool instant { get; }

        public StyleArgs(Selector selector, bool instant)
        {
            this.selector = selector;
            this.instant = instant;
        }
    }
}