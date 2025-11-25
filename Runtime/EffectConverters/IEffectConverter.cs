namespace Cdm.Figma.UI
{
    public interface IEffectConverter
    {
        bool CanConvert(FigmaNode node, Effect effect);
        void Convert(FigmaNode node, Effect effect);
    }
}