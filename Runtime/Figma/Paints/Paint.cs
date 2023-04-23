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
        public virtual string type { get; set; }
        
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
    
    public static class PaintType
    {
        public const string Solid = "SOLID";
        public const string GradientLinear = "GRADIENT_LINEAR";
        public const string GradientRadial = "GRADIENT_RADIAL";
        public const string GradientAngular = "GRADIENT_ANGULAR";
        public const string GradientDiamond = "GRADIENT_DIAMOND";
        public const string Image = "IMAGE";
    }
}