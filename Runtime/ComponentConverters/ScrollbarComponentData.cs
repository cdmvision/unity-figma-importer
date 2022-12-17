using System.Runtime.Serialization;
using UnityEngine.UI;

namespace Cdm.Figma.UI
{
    [DataContract]
    public class ScrollbarComponentData
    {
        [DataMember]
        public Scrollbar.Direction direction { get; set; } = Scrollbar.Direction.LeftToRight;
    }
}