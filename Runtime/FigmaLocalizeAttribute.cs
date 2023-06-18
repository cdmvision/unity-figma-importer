namespace Cdm.Figma.UI
{
    public class FigmaLocalizeAttribute : FigmaImporterAttribute
    {
        public string localizationKey { get; }

        public FigmaLocalizeAttribute(string localizationKey)
        {
            this.localizationKey = localizationKey;
        }
    }
}