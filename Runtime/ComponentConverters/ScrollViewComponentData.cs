using System.Runtime.Serialization;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    [DataContract]
    public class ScrollViewComponentData
    {
        [DataMember]
        public ScrollRect.ScrollbarVisibility horizontalVisibility { get; set; } =
            ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        
        [DataMember]
        public ScrollRect.ScrollbarVisibility verticalVisibility { get; set; } =
            ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
    }
}