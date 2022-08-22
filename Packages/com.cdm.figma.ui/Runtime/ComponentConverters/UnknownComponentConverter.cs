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
    }
}