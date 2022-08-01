using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public static class ToggleComponentState
    {
        public const string On = "On";
        public const string Off = "Off";
    }
    
    [RequireComponent(typeof(Toggle))]
    public class ToggleComponentVariantFilter : SelectableComponentVariantFilter
    {
        private Toggle _toggle;
        private bool _isToggleOn;
        
        protected override void Awake()
        {
            base.Awake();
            
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(Toggle_OnValueChanged);
        }

        protected override string GetSelector()
        {
            var selector = base.GetSelector();
            return $"{selector}:{toggleSelector}";
        }

        private string toggleSelector => _isToggleOn ? ToggleComponentState.On : ToggleComponentState.Off;

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateToggleState();
        }

        private void Toggle_OnValueChanged(bool isOn)
        {
            _isToggleOn = isOn;
            UpdateVariant();
        }
        
        private void UpdateToggleState()
        {
            if (_toggle != null)
            {
                _isToggleOn = _toggle.isOn;
                UpdateVariant();
            }
        }
    }
}