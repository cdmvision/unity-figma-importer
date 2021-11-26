using System.Globalization;
using System.Reflection;
using JsonSubTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Cdm.Figma
{
    internal class NonPublicPropertiesResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (member is PropertyInfo pi)
            {
                prop.Readable = (pi.GetMethod != null);
                prop.Writable = (pi.SetMethod != null);
            }

            return prop;
        }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            Converters =
            {
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal},
                new AffineTransformConverter(),
                JsonSubtypesConverterBuilder
                    .Of<Node>("type") // type property is only defined here
                    .RegisterSubtype<BooleanOperationNode>(NodeType.Boolean)
                    .RegisterSubtype<CanvasNode>(NodeType.Canvas)
                    .RegisterSubtype<ComponentNode>(NodeType.Component)
                    .RegisterSubtype<ComponentSetNode>(NodeType.ComponentSet)
                    .RegisterSubtype<DocumentNode>(NodeType.Document)
                    .RegisterSubtype<EllipseNode>(NodeType.Ellipse)
                    .RegisterSubtype<FrameNode>(NodeType.Frame)
                    .RegisterSubtype<GroupNode>(NodeType.Group)
                    .RegisterSubtype<InstanceNode>(NodeType.Instance)
                    .RegisterSubtype<RectangleNode>(NodeType.Rectangle)
                    .RegisterSubtype<RegularPolygonNode>(NodeType.RegularPolygon)
                    .RegisterSubtype<SliceNode>(NodeType.Slice)
                    .RegisterSubtype<StarNode>(NodeType.Star)
                    .RegisterSubtype<LineNode>(NodeType.Line)
                    .RegisterSubtype<TextNode>(NodeType.Text)
                    .RegisterSubtype<VectorNode>(NodeType.Vector)
                    .SetFallbackSubtype(typeof(Node))
                    .SerializeDiscriminatorProperty() // ask to serialize the type property
                    .Build()
            },
            NullValueHandling = NullValueHandling.Ignore,
        };
    }
}