using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Styles
{
    [RequireComponent(typeof(Selectable))]
    public class SelectableGroup : UIBehaviour, IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] 
        private bool _inheritCanvasGroupInteractable = true;

        public bool inheritCanvasGroupInteractable
        {
            get => _inheritCanvasGroupInteractable;
            set => _inheritCanvasGroupInteractable = value;
        }

        [SerializeField] 
        private bool _inheritSelectableInteractable = true;

        public bool inheritSelectableInteractable
        {
            get => _inheritSelectableInteractable;
            set => _inheritSelectableInteractable = value;
        }

        protected CanvasGroup canvasGroup { get; private set; }
        protected List<Style> styles { get; } = new List<Style>();

        protected Selectable selectable { get; private set; }
        protected bool isPointerInside { get; set; }
        protected bool isPointerDown { get; set; }
        protected bool hasSelection { get; set; }

        protected Selector currentSelector
        {
            get
            {
                if (!IsInteractable())
                    return Selector.Disabled;

                if (isPointerDown)
                    return Selector.Pressed;

                if (hasSelection)
                    return Selector.Selected;

                if (isPointerInside)
                    return Selector.Highlighted;

                return Selector.Normal;
            }
        }
        
        protected override void Awake()
        {
            base.Awake();

            selectable = GetComponent<Selectable>();
            canvasGroup = GetComponentInParent<CanvasGroup>();

            InitializeComponents(transform);
        }

        private void InitializeComponents(Transform node)
        {
            var selectableGroup = node.GetComponent<SelectableGroup>();
            if (selectableGroup == null || selectableGroup == this)
            {
                var styles2 = node.GetComponents<Style>();
                foreach (var style in styles2)
                {
                    InitializeStyle(style);
                    styles.Add(style);
                }
            }

            foreach (Transform child in node)
            {
                InitializeComponents(child);
            }
        }

        protected virtual void InitializeStyle(Style style)
        {
            style.interactable = IsInteractable();
        }

        protected virtual void Update()
        {
            // TODO: Optimize it.
            var interactable = IsInteractable();
            foreach (var style in styles)
            {
                style.interactable = interactable;
            }
        }

        private bool IsInteractable()
        {
            if (_inheritCanvasGroupInteractable && canvasGroup != null)
            {
                if (!canvasGroup.interactable)
                {
                    return false;
                }
            }

            if (_inheritSelectableInteractable && selectable != null)
            {
                return selectable.interactable;
            }

            return true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            
            SetCurrentSelectorForStyles();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            
            SetCurrentSelectorForStyles();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerInside = true;
            
            SetCurrentSelectorForStyles();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerInside = false;
            
            SetCurrentSelectorForStyles();
        }

        public void OnSelect(BaseEventData eventData)
        {
            hasSelection = true;
            SetCurrentSelectorForStyles();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            hasSelection = false;

            SetCurrentSelectorForStyles();
        }

        private void SetCurrentSelectorForStyles()
        {
            SetSelectorForStyles(currentSelector);
        }

        private void SetSelectorForStyles(Selector selector)
        {
            foreach (var style in styles)
            {
                style.Apply(selector);
            }
        }
        
#if UNITY_EDITOR
        /*protected override void OnValidate()
        {
            base.OnValidate();
            
            if (isActiveAndEnabled)
            {
                SetCurrentSelectorForStyles();
            }
        }*/
#endif

    }
}