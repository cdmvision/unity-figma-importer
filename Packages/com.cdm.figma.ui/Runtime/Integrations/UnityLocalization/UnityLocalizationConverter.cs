using Cdm.Figma.UI.Styles;
using UnityEngine;
using UnityEngine.Localization;

namespace Cdm.Figma.UI
{
    public class UnityLocalizationConverter : ILocalizationConverter
    {
        public bool CanConvert(FigmaText node)
        {
            return !string.IsNullOrEmpty(node.localizationKey);
        }

        public void Convert(FigmaText node)
        {
            var style = new UnityLocalizationStyle();
            style.localizedString.enabled = false;

            var localizationKey = node.localizationKey;
            if (!string.IsNullOrEmpty(localizationKey))
            {
                if (UnityLocalizationHelper.TryGetTableAndEntryReference(
                        localizationKey, out var tableReference, out var tableEntryReference))
                {
                    style.localizedString.SetValue(new LocalizedString(tableReference, tableEntryReference));
                }
                else
                {
                    Debug.LogWarning($"Localization key could not be mapped: {localizationKey}");
                }
            }

            node.styles.Add(style);
        }
    }
}