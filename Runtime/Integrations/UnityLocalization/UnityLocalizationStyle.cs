using System;
using Cdm.Figma.UI.Styles;
using Cdm.Figma.UI.Styles.Properties;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.Core.Formatting;

namespace Cdm.Figma.UI
{
    [Serializable]
    public class UnityLocalizationStyle : StyleWithSetter<UnityLocalizationStyleSetter>
    {
        public StylePropertyLocalizedString localizedString = new StylePropertyLocalizedString();

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
                    // Importing fails while smart strings are being calculated without args.
                    // So ignore the error in Editor.
                    try
                    {
                        stringEvent.StringReference = localizedString.value;
                    }
                    catch (FormattingException)
                    {
                    }
                }
#endif
            }
        }
    }
}