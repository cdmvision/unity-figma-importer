using System;
using Cdm.Figma.UI.Styles;
using Cdm.Figma.UI.Styles.Properties;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Core.Formatting;

namespace Cdm.Figma.UI
{
    [Serializable]
    public class UnityLocalizationStyle : StyleWithSetter<UnityLocalizationStyleSetter>
    {
        public StylePropertyLocalizedString localizedString = new StylePropertyLocalizedString();

#if UNITY_EDITOR
        private static bool _resolveFailureWarned;
#endif

        protected override void MergeTo(Style other, bool force)
        {
            if (other is UnityLocalizationStyle otherStyle)
            {
                OverwriteProperty(localizedString, otherStyle.localizedString, force);
            }
        }

        public override void SetStyle(GameObject gameObject, StyleArgs args)
        {
            var textComponent = gameObject.GetComponent<TMP_Text>();
            if (textComponent != null && localizedString.enabled)
            {
                var stringEvent = gameObject.GetComponent<LocalizeStringEvent>();

                if (stringEvent == null)
                {
                    stringEvent = gameObject.AddComponent<LocalizeStringEvent>();
                    UnityLocalizationHelper.AddUpdateStringEvent(textComponent, stringEvent);
                }
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
#endif
                    stringEvent.StringReference = localizedString.value;
                    stringEvent.RefreshString();
#if UNITY_EDITOR
                }
                else
                {
                    // Assigning the reference resolves the entry straight away, which an import
                    // has no use for. The reference is stored before the resolve is attempted,
                    // so what gets imported is the same whether or not the resolve throws.
                    try
                    {
                        stringEvent.StringReference = localizedString.value;
                    }
                    catch (FormattingException)
                    {
                        // Smart strings are formatted without their arguments during an import.
                    }
                    catch (Exception e)
                    {
                        // The database can return a table Unity has already destroyed, and that
                        // would otherwise take the whole import down. Resetting reloads the
                        // tables so the remaining text nodes do not hit the same thing.
                        LocalizationSettings.Instance.ResetState();

                        if (!_resolveFailureWarned)
                        {
                            _resolveFailureWarned = true;
                            Debug.LogWarning(
                                $"Localization state was reset while importing: {e.Message}");
                        }
                    }
                }
#endif
            }
        }
    }
}