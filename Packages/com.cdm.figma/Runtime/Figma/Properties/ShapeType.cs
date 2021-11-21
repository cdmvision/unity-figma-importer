using System;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// Geometric shape type.
    /// </summary>
    [Serializable]
    public enum ShapeType
    {
        [EnumMember(Value = "SQUARE")]
        Square,

        [EnumMember(Value = "ELLIPSE")]
        Ellipse,

        [EnumMember(Value = "ROUNDED_RECTANGLE")]
        RoundedRectangle,

        [EnumMember(Value = "DIAMOND")]
        Diamond,

        [EnumMember(Value = "TRIANGLE_DOWN")]
        TriangleDown,

        [EnumMember(Value = "PARALLELOGRAM_RIGHT")]
        ParallelogramRight,

        [EnumMember(Value = "PARALLELOGRAM_LEFT")]
        ParallelogramLeft,
    }
}