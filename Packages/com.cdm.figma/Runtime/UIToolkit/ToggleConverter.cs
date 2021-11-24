using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    public static class ToggleState
    {
        public const string On = "On";
        public const string Off = "Off";
    }
    
    [CreateAssetMenu(fileName = nameof(ToggleConverter), menuName = AssetMenuRoot + "Toggle")]
    public class ToggleConverter : ComponentConverter
    {
        public static class States
        {
            public const string DefaultOn = "Default/On";
            public const string HoverOn = "Hover/On";
            public const string PressOn = "Press/On";
            public const string DisabledOn = "Disabled/On";
            
            public const string DefaultOff = "Default/Off";
            public const string HoverOff = "Hover/Off";
            public const string PressOff = "Press/Off";
            public const string DisabledOff = "Disabled/Off";
        }
        
        protected override string GetDefaultTypeId()
        {
            return "Toggle";
        }
        
        protected override ISet<ComponentProperty> GetVariants()
        {
            return new HashSet<ComponentProperty>()
            {
                new ComponentProperty()
                {
                    key = "State",
                    variants = new ComponentVariant[]
                    {
                        new ComponentVariant(ComponentState.Default, ComponentState.Default),
                        new ComponentVariant(ComponentState.Hover, ComponentState.Hover),
                        new ComponentVariant(ComponentState.Press, ComponentState.Press),
                        new ComponentVariant(ComponentState.Disabled, ComponentState.Disabled),
                    }
                },
                new ComponentProperty()
                {
                    key = "Checked",
                    variants = new ComponentVariant[]
                    {
                        new ComponentVariant(ToggleState.On, ToggleState.On),
                        new ComponentVariant(ToggleState.Off, ToggleState.Off),
                    }
                }
            };
        }

        public override VisualTreeAsset Convert(UIToolkitFigmaImporter importer, FigmaFile file, Node node)
        {
            throw new System.NotImplementedException();
        }
    }
}