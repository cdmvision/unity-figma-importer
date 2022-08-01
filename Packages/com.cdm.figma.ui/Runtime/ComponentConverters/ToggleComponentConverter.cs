using Cdm.Figma.UI.Styles;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class ToggleComponentConverter : ComponentConverter<Toggle>
    {
        private const string TypeID = "Toggle";
        
        public ToggleComponentConverter()
        {
            //properties = new List<ComponentProperty>() { stateProperty };
        }
        
        protected override bool TryGetSelector(string[] variant, out Selector selector)
        {
            selector = Selector.Normal;
            return false;
        }

        protected override bool CanConvertType(string typeID)
        {
            return typeID == TypeID;
        }
    }
}