using System;
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

        protected Selectable selectable { get; private set; }
        protected CanvasGroup canvasGroup { get; private set; }
        protected List<Style> styles { get; } = new List<Style>();

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
            style.OnInteractableChanged(IsInteractable());
        }

        protected virtual void Update()
        {
            // TODO: Optimize it.
            var interactable = IsInteractable();
            foreach (var style in styles)
            {
                style.OnInteractableChanged(interactable);
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
            foreach (var style in styles)
            {
                style.OnPointerDown(eventData);
            }
            
            pointerDown?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            foreach (var style in styles)
            {
                style.OnPointerUp(eventData);
            }
            
            pointerUp?.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            foreach (var style in styles)
            {
                style.OnPointerEnter(eventData);
            }
            
            pointerEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (var style in styles)
            {
                style.OnPointerExit(eventData);
            }
            
            pointerExit?.Invoke(eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            foreach (var style in styles)
            {
                style.OnSelect(eventData);
            }
            
            selected?.Invoke(eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            foreach (var style in styles)
            {
                style.OnDeselect(eventData);
            }
            
            deselected?.Invoke(eventData);
        }

        public event Action<PointerEventData> pointerDown;
        public event Action<PointerEventData> pointerUp;
        public event Action<PointerEventData> pointerEnter;
        public event Action<PointerEventData> pointerExit;
        public event Action<BaseEventData> selected;
        public event Action<BaseEventData> deselected;
    }
}