using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public static class SelectableComponentState
    {
        public const string Default = "Default";
        public const string Hover = "Hover";
        public const string Press = "Press";
        public const string Disabled = "Disabled";
    }

    [RequireComponent(typeof(Selectable))]
    public class SelectableComponentVariantFilter : ComponentVariantFilter,
        IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
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
            if (!interactable)
                return SelectableComponentState.Disabled;

            if (isPointerDown)
                return SelectableComponentState.Press;

            if (isPointerInside)
                return SelectableComponentState.Hover;

            return SelectableComponentState.Default;
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
    }
}