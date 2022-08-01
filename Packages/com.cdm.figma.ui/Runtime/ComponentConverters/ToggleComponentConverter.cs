using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class ToggleComponentConverter : SelectableComponentConverter<Toggle, ToggleComponentVariantFilter>
    {
        private const string TypeID = "Toggle";

        protected const int Off = 0;
        protected const int On = 1;
        
        public ComponentProperty checkedProperty { get; } = new ComponentProperty("Checked", new[]
        {
            ToggleComponentState.Off,
            ToggleComponentState.On
        });
        
        public ToggleComponentConverter()
        {
            properties.Add(checkedProperty);
        }
        
        protected override bool TryGetSelector(string[] variant, out string selector)
        {
            if (!base.TryGetSelector(variant, out selector))
                return false;
            
            if (IsSameVariant(variant, checkedProperty.ToString(Off)))
            {
                selector += $":{ToggleComponentState.Off}";
                return true;
            }

            if (IsSameVariant(variant, checkedProperty.ToString(On)))
            {
                selector += $":{ToggleComponentState.On}";
                return true;
            }

            selector = null;
            return false;
        }

        protected override bool CanConvertType(string typeID)
        {
            return typeID == TypeID;
        }
    }
}