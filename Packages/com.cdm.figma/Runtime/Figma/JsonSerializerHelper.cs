using System.Globalization;
using System.Reflection;
using JsonSubTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Cdm.Figma
{
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
                    //GetGroupNodeConverter(),
                    //GetFrameNodeConverter(),
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
                .RegisterSubtype<PageNode>(NodeType.Page)
                .RegisterSubtype<ComponentNode>(NodeType.Component)
                .RegisterSubtype<ComponentSetNode>(NodeType.ComponentSet)
                .RegisterSubtype<DocumentNode>(NodeType.Document)
                .RegisterSubtype<EllipseNode>(NodeType.Ellipse)
                .RegisterSubtype<FrameNode>(NodeType.Frame)
                .RegisterSubtype<GroupNode>(NodeType.Group)
                .RegisterSubtype<InstanceNode>(NodeType.Instance)
                .RegisterSubtype<RectangleNode>(NodeType.Rectangle)
                .RegisterSubtype<PolygonNode>(NodeType.Polygon)
                .RegisterSubtype<SliceNode>(NodeType.Slice)
                .RegisterSubtype<StarNode>(NodeType.Star)
                .RegisterSubtype<LineNode>(NodeType.Line)
                .RegisterSubtype<TextNode>(NodeType.Text)
                .RegisterSubtype<VectorNode>(NodeType.Vector)
                .SetFallbackSubtype(typeof(Node))
                .SerializeDiscriminatorProperty()
                .Build();
        }

        private static JsonConverter GetGroupNodeConverter()
        {
            return JsonSubtypesConverterBuilder
                .Of<GroupNode>("type")
                .RegisterSubtype<FrameNode>(NodeType.Frame)
                .SetFallbackSubtype(typeof(GroupNode))
                .SerializeDiscriminatorProperty()
                .Build();
        }
        
        private static JsonConverter GetFrameNodeConverter()
        {
            return JsonSubtypesConverterBuilder
                .Of<FrameNode>("type")
                .RegisterSubtype<InstanceNode>(NodeType.Instance)
                .SetFallbackSubtype(typeof(FrameNode))
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
}