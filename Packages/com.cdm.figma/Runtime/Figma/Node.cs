using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace Cdm.Figma
{
    [Serializable]
    public class Node
    {
        public string name;
        public string id;
        public NodeType type;
        [SerializeReference]
        public Node[] children;
        public bool visible;
        public Color backgroundColor;
        public ExportSetting[] exportSettings;
        public Rectangle absoluteBoundingBox;
        public BlendMode? blendMode;
        public bool? clipsContent;
        public LayoutConstraint constraints;
        public Effect[] effects;
        public bool? isMask;
        public LayoutGrid[] layoutGrids;
        public float? opacity;
        public bool? preserveRatio;
        public string transitionNodeId;
        public Paint[] fills;
        public StrokeAlign? strokeAlign;
        public Paint[] strokes;
        public float? strokeWeight;
        public float? cornerRadius;
        public string characters;
        public float[] characterStyleOverrides;
        public TypeStyle style;
        public TypeStyle styleOverrideTable;
        public string description;
        public string componentId;
    }
    
    /// <summary>
    /// The type of the node, refer to table below for details.
    /// </summary>
    [Serializable]
    public enum NodeType
    {
        [EnumMember(Value = "BOOLEAN_OPERATION")]
        Boolean,

        [EnumMember(Value = "CANVAS")]
        Canvas,

        [EnumMember(Value = "COMPONENT")]
        Component,

        [EnumMember(Value = "COMPONENT_SET")]
        ComponentSet,

        [EnumMember(Value = "DOCUMENT")]
        Document,

        [EnumMember(Value = "ELLIPSE")]
        Ellipse,

        [EnumMember(Value = "FRAME")]
        Frame,

        [EnumMember(Value = "GROUP")]
        Group,

        [EnumMember(Value = "INSTANCE")]
        Instance,

        [EnumMember(Value = "LINE")]
        Line,

        [EnumMember(Value = "RECTANGLE")]
        Rectangle,

        [EnumMember(Value = "REGULAR_POLYGON")]
        RegularPolygon,

        [EnumMember(Value = "SLICE")]
        Slice,

        [EnumMember(Value = "STAR")]
        Star,

        [EnumMember(Value = "TEXT")]
        Text,

        [EnumMember(Value = "VECTOR")]
        Vector
    }
}