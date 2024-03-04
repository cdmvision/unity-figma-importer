using Cdm.Figma.UI;
using UnityEngine;

namespace Cdm.Figma.Examples
{
    [FigmaNode("@VectorsContent")]
    public class VectorsContent : ScrollableContent
    {
        [FigmaNode("@Vectors")]
        [SerializeField]
        private RectTransform _vectors;

        protected override RectTransform GetContent()
        {
            return _vectors;
        }
    }
}