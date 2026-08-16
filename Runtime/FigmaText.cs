using UnityEngine;

namespace Cdm.Figma.UI
{
    public class FigmaText : FigmaNode
    {
        [SerializeField]
        private string _localizationKey;

        /// <summary>
        /// The localization key set by the user in Figma editor using Unity Figma Plugin. It is used to localize text
        /// by the key that points to a table entry.
        /// </summary>
        public string localizationKey
        {
            get => _localizationKey;
            internal set => _localizationKey = value;
        }
    }
}