using System;
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
            if (TryGetLocalizedValue(localizationKey, out var value))
            {
                style.localizedString.SetValue((LocalizedString)value);
            }
            else
            {
                Debug.LogWarning($"Localization key could not be mapped: {localizationKey}");
            }

            node.styles.Add(style);
        }

        public bool CanBind(Type type)
        {
            return type == typeof(LocalizedString);
        }

        public bool TryGetLocalizedValue(string key, out object value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (UnityLocalizationHelper.TryGetTableAndEntryReference(
                        key, out var tableReference, out var tableEntryReference))
                {
                    value = new LocalizedString(tableReference, tableEntryReference);
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}