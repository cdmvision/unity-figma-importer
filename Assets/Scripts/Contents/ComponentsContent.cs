using Cdm.Figma.UI;
using UnityEngine;

namespace Cdm.Figma.Examples
{
    [FigmaNode("@ComponentsContent")]
    public class ComponentsContent : ScrollableContent
    {
        [FigmaNode("@Components")]
        [SerializeField]
        private RectTransform _components;
        
        protected override RectTransform GetContent()
        {
            return _components;
        }
    }
}