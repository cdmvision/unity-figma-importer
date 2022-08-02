using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class ButtonComponentConverter : SelectableComponentConverter<Button, SelectableComponentVariantFilter>
    {
        protected override bool CanConvertType(string typeID)
        {
            return typeID == "Button";
        }
    }
}