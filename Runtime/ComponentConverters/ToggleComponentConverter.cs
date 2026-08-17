using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public static class ComponentPropertyChecked
    {
        public static readonly ComponentProperty Off = new ComponentProperty("Checked", "Off");
        public static readonly ComponentProperty On = new ComponentProperty("Checked", "On");
    }

    public abstract class ToggleComponentConverter<TToggle, TComponentVariantFilter>
        : SelectableComponentConverter<TToggle, TComponentVariantFilter>
        where TToggle : Toggle
        where TComponentVariantFilter : ToggleComponentVariantFilter
    {
        protected override bool TryGetSelector(string[] variant, out string selector)
        {
            if (!base.TryGetSelector(variant, out selector))
                return false;

            if (!TryGetSelector(variant, ComponentPropertyChecked.Off, ref selector) &&
                !TryGetSelector(variant, ComponentPropertyChecked.On, ref selector))
            {
                return false;
            }

            return true;
        }
    }

    public class ToggleComponentConverter : ToggleComponentConverter<Toggle, ToggleComponentVariantFilter>
    {
        protected override bool CanConvertType(string typeId)
        {
            return typeId == "Toggle";
        }
    }
}