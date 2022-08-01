using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class ButtonComponentConverter : SelectableComponentConverter<Button, SelectableComponentVariantFilter>
    {
        private const string TypeID = "Button";
        
        protected override bool CanConvertType(string typeID)
        {
            return typeID == TypeID;
        }
    }
}