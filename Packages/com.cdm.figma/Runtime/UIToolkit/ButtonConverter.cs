using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma.UIToolkit
{
    [CreateAssetMenu(fileName = nameof(ButtonConverter), menuName = AssetMenuRoot + "Button")]
    public class ButtonConverter : ComponentConverter
    {
        public VisualTreeAsset Convert(FigmaFile file, Node node)
        {
            throw new System.NotImplementedException();
        }

        protected override string GetDefaultTypeId()
        {
            return "Button";
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
                }
            };
        }

        public override VisualTreeAsset Convert(UIToolkitFigmaImporter importer, FigmaFile file, Node node)
        {
            throw new System.NotImplementedException();
        }
    }
}