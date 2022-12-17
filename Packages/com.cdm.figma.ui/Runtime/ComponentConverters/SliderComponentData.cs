using System.Runtime.Serialization;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    [DataContract]
    public class SliderComponentData
    {
        [DataMember]
        public Slider.Direction direction { get; set; } = Slider.Direction.LeftToRight;
    }
}