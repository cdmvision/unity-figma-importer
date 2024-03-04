using Cdm.Figma.UI;
using UnityEngine;

namespace Cdm.Figma.Examples
{
    [FigmaNode("@TypographyContent")]
    public class TypographyContent : ScrollableContent
    {
        [FigmaNode("@Fonts")]
        [SerializeField]
        private RectTransform _fonts;

        protected override RectTransform GetContent()
        {
            return _fonts;
        }
    }
}