using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class SolidPaint : Paint
    {
        public override PaintType type => PaintType.Solid;
        
        /// <summary>
        /// Solid color of the paint.
        /// </summary>
        [DataMember(Name = "color")]
        public Color color { get; set; }
    }
}