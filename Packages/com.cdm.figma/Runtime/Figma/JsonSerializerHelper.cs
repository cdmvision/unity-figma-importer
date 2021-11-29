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

    public static class JsonSerializerHelper
    {
        public static readonly JsonSerializerSettings Settings = CreateSettings();

        public static JsonSerializerSettings CreateSettings()
        {
            return new JsonSerializerSettings()
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                Converters =
                {
                    new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal},
                    new AffineTransformConverter(),
                    GetNodeConverter(),
                    GetEffectConverter(),
                    GetPaintConverter()
                },
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        private static JsonConverter GetNodeConverter()
        {
            return JsonSubtypesConverterBuilder
                .Of<Node>("type")
                .RegisterSubtype<BooleanNode>(NodeType.Boolean)
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
                .SerializeDiscriminatorProperty()
                .Build();
        }

        private static JsonConverter GetEffectConverter()
        {
            return JsonSubtypesConverterBuilder
                .Of<Effect>("type")
                .RegisterSubtype<LayerBlurEffect>(EffectType.LayerBlur)
                .RegisterSubtype<BackgroundBlurEffect>(EffectType.BackgroundBlur)
                .RegisterSubtype<DropShadowEffect>(EffectType.DropShadow)
                .RegisterSubtype<InnerShadowEffect>(EffectType.InnerShadow)
                .SetFallbackSubtype(typeof(Effect))
                .SerializeDiscriminatorProperty()
                .Build();
        }

        private static JsonConverter GetPaintConverter()
        {
            return JsonSubtypesConverterBuilder
                .Of<Paint>("type")
                .RegisterSubtype<SolidPaint>(PaintType.Solid)
                .RegisterSubtype<LinearGradientPaint>(PaintType.GradientLinear)
                .RegisterSubtype<RadialGradientPaint>(PaintType.GradientRadial)
                .RegisterSubtype<AngularGradientPaint>(PaintType.GradientAngular)
                .RegisterSubtype<DiamondGradientPaint>(PaintType.GradientDiamond)
                .RegisterSubtype<ImagePaint>(PaintType.Image)
                .SetFallbackSubtype(typeof(Paint))
                .SerializeDiscriminatorProperty()
                .Build();
        }
    }
}