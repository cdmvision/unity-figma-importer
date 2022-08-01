using Cdm.Figma.UI.Styles;

namespace Cdm.Figma.UI
{
    public class UnknownComponentConverter : ComponentConverter
    {
        public UnknownComponentConverter()
        {
        }

        protected override bool CanConvertType(string typeID)
        {
            return string.IsNullOrWhiteSpace(typeID);
        }

        protected override bool TryGetSelector(string[] variant, out Selector selector)
        {
            selector = Selector.Normal;
            return false;
        }
    }
}