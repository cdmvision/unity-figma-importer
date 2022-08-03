﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    [RequireComponent(typeof(Selectable))]
    public class SelectableComponentVariantFilter : ComponentVariantFilter,
        IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, 
        ISelectHandler, IDeselectHandler
    {
        [SerializeField]
        private bool _inheritCanvasGroupInteractable = true;

        public bool inheritCanvasGroupInteractable
        {
            get => _inheritCanvasGroupInteractable;
            set => _inheritCanvasGroupInteractable = value;
        }

        protected Selectable selectable { get; private set; }
        protected CanvasGroup canvasGroup { get; private set; }
        protected bool isPointerInside { get; set; }
        protected bool isPointerDown { get; set; }
        protected bool interactable { get; set; }

        protected bool hasSelection { get; set; }
        public bool isSelectable { get; set; }

        protected override void Awake()
        {
            base.Awake();

            canvasGroup = GetComponent<CanvasGroup>();
            selectable = GetComponent<Selectable>();
            interactable = IsInteractable();
        }

        protected virtual void Update()
        {
            var isInteractable = IsInteractable();
            if (isInteractable != interactable)
            {
                interactable = isInteractable;
                UpdateVariant();
            }
        }

        protected override string GetSelector()
        {
            var selector = ComponentPropertyState.Default.value;

            if (!interactable)
            {
                selector = ComponentPropertyState.Disabled.value;
            }
            else if (isPointerDown)
            {
                selector = ComponentPropertyState.Press.value;
            }
            else if (isPointerInside)
            {
                selector = ComponentPropertyState.Hover.value;
            }

            if (isSelectable)
            {
                if (hasSelection)
                {
                    selector += $":{ComponentPropertySelected.On.value}";
                }
                else
                {
                    selector += $":{ComponentPropertySelected.Off.value}";
                }
            }

            return selector;
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

            return selectable.interactable;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log($"OnPointerDown: {name}");
            isPointerDown = true;
            UpdateVariant();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log($"OnPointerUp: {name}");
            isPointerDown = false;
            UpdateVariant();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log($"OnPointerEnter: {name}");
            isPointerInside = true;
            UpdateVariant();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log($"OnPointerExit: {name}");
            isPointerInside = false;
            UpdateVariant();
        }

        public void OnSelect(BaseEventData eventData)
        {
            //Debug.Log($"OnSelect: {name}");
            hasSelection = true;
            UpdateVariant();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            //Debug.Log($"OnDeselect: {name}");
            hasSelection = false;
            UpdateVariant();
        }
    }
}