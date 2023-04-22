using System;
using Cdm.Figma.UI.Styles;
using Cdm.Figma.UI.Styles.Properties;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Cdm.Figma.UI
{
    [Serializable]
    public class UnityLocalizationStyle : StyleWithSetter<UnityLocalizationStyleSetter>
    {
        public StylePropertyLocalizedString localizedString = new StylePropertyLocalizedString();

        protected override void MergeTo(Styles.Style other, bool force)
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
                
                stringEvent.StringReference = localizedString.value;
                stringEvent.RefreshString();
            }
        }
    }
}