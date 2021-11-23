using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
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

        protected override ISet<ComponentState> GetStates()
        {
            return new HashSet<ComponentState>()
            {
                new ComponentState(States.DefaultOn, States.DefaultOn),
                new ComponentState(States.HoverOn, States.HoverOn),
                new ComponentState(States.PressOn, States.PressOn),
                new ComponentState(States.DisabledOn, States.DisabledOn),
                new ComponentState(States.DefaultOff, States.DefaultOff),
                new ComponentState(States.HoverOff, States.HoverOff),
                new ComponentState(States.PressOff, States.PressOff),
                new ComponentState(States.DisabledOff, States.DisabledOff)
            };
        }

        public override VisualTreeAsset Convert(UIToolkitFigmaImporter importer, FigmaFile file, Node node)
        {
            throw new System.NotImplementedException();
        }
    }
}