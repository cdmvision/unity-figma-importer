using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public static class ComponentPropertyChecked
    {
        public static readonly ComponentProperty Off = new ComponentProperty("Checked", "Off");
        public static readonly ComponentProperty On = new ComponentProperty("Checked", "On");
    }
    
    public class ToggleComponentConverter : SelectableComponentConverter<Toggle, ToggleComponentVariantFilter>
    {
        private const string TypeID = "Toggle";
        
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

        protected override bool CanConvertType(string typeID)
        {
            return typeID == TypeID;
        }
    }
}