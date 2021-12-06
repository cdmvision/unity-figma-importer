using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public static class ToggleState
    {
        public const string On = "On";
        public const string Off = "Off";
    }

    public class ToggleComponentConverter : ComponentConverter<Toggle>
    {
        private const int Default = 0;
        private const int Hover = 1;
        private const int Press = 2;
        private const int Disabled = 3;
        
        private const int Off = 0;
        private const int On = 1;
        
        public ComponentProperty stateProperty { get; } = new ComponentProperty("State", new[]
        {
            ComponentState.Default,
            ComponentState.Hover,
            ComponentState.Press,
            ComponentState.Disabled,
        });

        public ComponentProperty checkedProperty { get; } = new ComponentProperty("Checked", new[]
        {
            ToggleState.Off,
            ToggleState.On
        });
        
        
        public ToggleComponentConverter()
        {
            typeId = "Toggle";
            properties = new List<ComponentProperty>() { stateProperty, checkedProperty,};
        }
        
        protected override bool TryGetSelector(string[] variant, out string selector)
        {
            if (SameVariant(variant, stateProperty.ToString(Default), checkedProperty.ToString(Off)))
            {
                selector = "";
                return true;
            }

            if (SameVariant(variant, stateProperty.ToString(Hover), checkedProperty.ToString(Off)))
            {
                selector = ":hover";
                return true;
            }

            if (SameVariant(variant, stateProperty.ToString(Press), checkedProperty.ToString(Off)))
            {
                selector = ":active";
                return true;
            }

            if (SameVariant(variant, stateProperty.ToString(Disabled), checkedProperty.ToString(Off)))
            {
                selector = ":disabled";
                return true;
            }

            if (SameVariant(variant, stateProperty.ToString(Default), checkedProperty.ToString(On)))
            {
                selector = ":checked";
                return true;
            }

            if (SameVariant(variant, stateProperty.ToString(Hover), checkedProperty.ToString(On)))
            {
                selector = ":checked:hover";
                return true;
            }

            if (SameVariant(variant, stateProperty.ToString(Press), checkedProperty.ToString(On)))
            {
                selector = ":checked:active";
                return true;
            }

            if (SameVariant(variant, stateProperty.ToString(Disabled), checkedProperty.ToString(On)))
            {
                selector = ":checked:disabled";
                return true;
            }

            selector = null;
            return false;
        }
    }
}