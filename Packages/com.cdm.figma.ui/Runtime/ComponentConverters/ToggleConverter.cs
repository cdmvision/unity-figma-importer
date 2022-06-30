using Cdm.Figma.UI.Styles;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    public class ToggleConverter : ComponentConverter<Toggle>
    {
        protected override bool TryGetSelector(string[] variant, out Selector selector)
        {
            selector = Selector.Normal;
            return false;
        }
    }
}