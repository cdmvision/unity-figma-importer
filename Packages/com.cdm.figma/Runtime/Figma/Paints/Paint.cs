using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// A solid color, gradient, or image texture that can be applied as fills or strokes.
    /// </summary>
    [DataContract]
    public class Paint
    {
        /// <summary>
        /// Type of paint as a string enum
        /// </summary>
        /// <seealso cref="PaintType"/>
        [DataMember(Name = "type", IsRequired = true)]
        public virtual PaintType type { get; set; }
        
        /// <summary>
        /// Is the paint enabled?
        /// </summary>
        [DataMember(Name = "visible")]
        public bool visible { get; set; } = true;

        /// <summary>
        /// Overall opacity of paint (colors within the paint can also have opacity
        /// values which would blend with this)
        /// </summary>
        [DataMember(Name = "opacity")]
        public float opacity { get; set; } = 1f;

        /// <summary>
        /// How this node blends with nodes behind it in the scene.
        /// </summary>
        /// <seealso cref="BlendMode"/>
        [DataMember(Name = "blendMode")]
        public BlendMode blendMode { get; set; } = BlendMode.Normal;
    }

    [DataContract]
    public enum PaintType
    {
        [EnumMember(Value = "SOLID")]
        Solid,
        
        [EnumMember(Value = "GRADIENT_LINEAR")]
        GradientLinear,
        
        [EnumMember(Value = "GRADIENT_RADIAL")]
        GradientRadial,
        
        [EnumMember(Value = "GRADIENT_ANGULAR")]
        GradientAngular,
        
        [EnumMember(Value = "GRADIENT_DIAMOND")]
        GradientDiamond,
        
        [EnumMember(Value = "IMAGE")]
        Image
    }
}