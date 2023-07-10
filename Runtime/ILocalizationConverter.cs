using System;

namespace Cdm.Figma.UI
{
    public interface ILocalizationConverter
    {
        bool CanConvert(FigmaText node);
        void Convert(FigmaText node);

        bool CanBind(Type type);
        bool TryGetLocalizedValue(string key, out object value);
    }
}