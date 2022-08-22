namespace Cdm.Figma.UI
{
    public class UnknownComponentConverter : ComponentConverter
    {
        public UnknownComponentConverter()
        {
        }

        protected override bool CanConvertType(string typeId)
        {
            return string.IsNullOrWhiteSpace(typeId);
        }
    }
}