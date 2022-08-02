using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
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

            if (_isToggleOn)
            {
                selector += $":{ComponentPropertyChecked.On.value}";
            }
            else
            {
                selector += $":{ComponentPropertyChecked.Off.value}";
            }

            return selector;
        }

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