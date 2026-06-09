using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;

namespace Cdm.Figma.UI
{
    public class UnityLocalizationHelper
    {
        public static bool TryGetTableAndEntryReference(string localizationKey, 
            out TableReference tableReference, out TableEntryReference tableEntryReference)
        {
            const char delimiter = '/';

            var tokens = localizationKey.Split(delimiter);

            if (tokens.Length != 2 || string.IsNullOrWhiteSpace(tokens[0]) || string.IsNullOrWhiteSpace(tokens[1]))
            {
                tableReference = default;
                tableEntryReference = default;
                return false;
            }

            tableReference = tokens[0];
            tableEntryReference = tokens[1];
            return true;
        }
        
        public static void BindLocalizeString(TMP_Text text, out LocalizeStringEvent stringEvent)
        {
            if (text != null)
            {
                stringEvent = text.GetComponent<LocalizeStringEvent>();

                if (stringEvent == null)
                {
                    stringEvent = text.gameObject.AddComponent<LocalizeStringEvent>();
                    AddUpdateStringEvent(text, stringEvent);
                }
            }
            else
            {
                stringEvent = null;
            }
        }

        public static void AddUpdateStringEvent(TMP_Text text, LocalizeStringEvent stringEvent)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                stringEvent.OnUpdateString.AddListener(s => text.text = s);
#if UNITY_EDITOR
            }
            else
            {
                var setStringMethod = text.GetType().GetProperty("text")?.GetSetMethod();
                if (setStringMethod != null)
                {
                    var methodDelegate =
                        Delegate.CreateDelegate(typeof(UnityAction<string>), text, setStringMethod) as
                            UnityAction<string>;
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(stringEvent.OnUpdateString,
                        methodDelegate);
                }
            }
#endif
        }
    }
}