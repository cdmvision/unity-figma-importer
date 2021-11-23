using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(ButtonConverter), menuName = AssetMenuRoot + "Button")]
    public class ButtonConverter : ComponentConverter
    {
        public static class States
        {
            public const string Default = "Default";
            public const string Hover = "Hover";
            public const string Press = "Press";
            public const string Disabled = "Disabled";
        }
        
        public VisualTreeAsset Convert(FigmaFile file, Node node)
        {
            throw new System.NotImplementedException();
        }

        protected override string GetDefaultTypeId()
        {
            return "Button";
        }
        
        protected override ISet<ComponentState> GetStates()
        {
            return new HashSet<ComponentState>()
            {
                new ComponentState(States.Default, States.Default),
                new ComponentState(States.Hover, States.Hover),
                new ComponentState(States.Press, States.Press),
                new ComponentState(States.Disabled, States.Disabled),
            };
        }

        public override VisualTreeAsset Convert(UIToolkitFigmaImporter importer, FigmaFile file, Node node)
        {
            throw new System.NotImplementedException();
        }
    }
}