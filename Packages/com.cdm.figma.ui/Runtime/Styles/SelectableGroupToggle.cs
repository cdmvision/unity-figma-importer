using UnityEngine;
using UnityEngine.UI;

namespace Cdm.Figma.UI.Styles
{
    [RequireComponent(typeof(Toggle))]
    public class SelectableGroupToggle : SelectableGroup
    {
        private Toggle _toggle;
        
        protected override void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(Toggle_OnValueChanged);
            base.Awake();
        }

        protected override void InitializeStyle(StyleSetter styleSetter)
        {
            base.InitializeStyle(styleSetter);
            
            /*if (style is StyleSetterToggle s)
            {
                s.isOn = _toggle.isOn;   
            }*/
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            UpdateToggleState();
        }

        protected override void Update()
        {
            base.Update();

            UpdateToggleState();
        }

        private void Toggle_OnValueChanged(bool isOn)
        {
            UpdateStyles(isOn);
        }
        
        private void UpdateToggleState()
        {
            if (_toggle != null)
            {
                UpdateStyles(_toggle.isOn);
            }
        }

        private void UpdateStyles(bool isOn)
        {
            /*foreach (var selectorStyle in selectorStyles)
            {
                if (selectorStyle is StyleSetterToggle s)
                {
                    s.isOn = isOn;   
                }
            }*/
        }
    }
}