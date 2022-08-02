using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public static class ComponentPropertyState
    {
        public static readonly ComponentProperty Default = new ComponentProperty("State", "Default");
        public static readonly ComponentProperty Hover = new ComponentProperty("State", "Hover");
        public static readonly ComponentProperty Press = new ComponentProperty("State", "Press");
        public static readonly ComponentProperty Disabled = new ComponentProperty("State", "Disabled");
    }

    public static class ComponentPropertySelected
    {
        public static readonly ComponentProperty Off = new ComponentProperty("Selected", "Off");
        public static readonly ComponentProperty On = new ComponentProperty("Selected", "On");
    }

    public abstract class SelectableComponentConverter<TComponent, TComponentVariantFilter> :
        ComponentConverter<TComponent, TComponentVariantFilter>
        where TComponent : Selectable
        where TComponentVariantFilter : SelectableComponentVariantFilter
    {
        protected bool isSelectable { get; set; } = false;
        
        protected override bool TryGetSelector(string[] variant, out string selector)
        {
            selector = "";

            if (!TryGetSelector(variant, ComponentPropertyState.Default, ref selector) &&
                !TryGetSelector(variant, ComponentPropertyState.Hover, ref selector) &&
                !TryGetSelector(variant, ComponentPropertyState.Press, ref selector) &&
                !TryGetSelector(variant, ComponentPropertyState.Disabled, ref selector))
            {
                return false;
            }

            if (isSelectable)
            {
                if (!TryGetSelector(variant, ComponentPropertySelected.Off, ref selector) &&
                    !TryGetSelector(variant, ComponentPropertySelected.On, ref selector))
                {
                    return false;
                }    
            }

            return true;
        }
    }
}