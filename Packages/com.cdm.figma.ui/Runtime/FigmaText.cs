using UnityEngine;

namespace Cdm.Figma.UI
{
    public class FigmaText : FigmaNode
    {
        [SerializeField]
        private string _localizationKey;

        public string localizationKey
        {
            get => _localizationKey;
            internal set => _localizationKey = value;
        }
    }
}