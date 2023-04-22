namespace Cdm.Figma.UI
{
    public interface ILocalizationConverter
    {
        bool CanConvert(FigmaText node);
        void Convert(FigmaText node);
    }
}