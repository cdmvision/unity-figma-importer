//#define DEBUG_SVG_STRING

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.VectorGraphics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cdm.Figma.Utils
{
    public enum SpriteGenerateType
    {
        Auto,
        Path,
        Rectangle
    }

    public struct SpriteGenerateOptions
    {
        public VectorUtils.TessellationOptions tessellationOptions { get; set; }

        public FilterMode filterMode { get; set; }
        public TextureWrapMode wrapMode { get; set; }
        public int minTextureSize { get; set; }
        public int maxTextureSize { get; set; }
        public int sampleCount { get; set; }
        public ushort gradientResolution { get; set; }
        public float pixelsPerUnit { get; set; }
        public float scaleFactor { get; set; }
        public SceneNode overrideNode { get; set; }

        public static SpriteGenerateOptions GetDefault()
        {
            return new SpriteGenerateOptions()
            {
                tessellationOptions = new VectorUtils.TessellationOptions()
                {
                    StepDistance = 1.0f,
                    MaxCordDeviation = 0.5f,
                    MaxTanAngleDeviation = 0.1f,
                    SamplingStepSize = 0.01f
                },
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                minTextureSize = 64,
                maxTextureSize = 256,
                sampleCount = 4,
                gradientResolution = 128,
                pixelsPerUnit = 100f,
                scaleFactor = 1f,
                overrideNode = null
            };
        }
    }

    public class NodeSpriteGenerator
    {
        /// <summary>
        /// Generates a sprite from the scene node.
        /// </summary>
        public static Sprite GenerateSprite(FigmaFile file, SceneNode node, SpriteGenerateType spriteType,
            SpriteGenerateOptions? options = null)
        {
            options ??= SpriteGenerateOptions.GetDefault();

            if (HasImageFill(node))
            {
                var imageSprite = GenerateSpriteFromImage(file, node, options.Value);
                if (imageSprite != null)
                    return imageSprite;
            }

            var svg = GenerateSpriteSvg(node, options.Value.overrideNode);
            var sprite = GenerateSprite(node, svg, spriteType, options);
            //Debug.Log($"{node}, [Sprite Generated:{(sprite != null ? "YES": "NO")}]: {svg}");

            return sprite;
        }

        /// <summary>
        /// Generates SVG string from the scene node.
        /// </summary>
        public static string GenerateSpriteSvg(SceneNode node, SceneNode overrideNode)
        {
            return GenerateSvgFromPath(node, overrideNode);
        }

        /// <summary>
        /// Generates a sprite from the scene node and the SVG string given.
        /// </summary>
        public static Sprite GenerateSprite(SceneNode node, string svg, SpriteGenerateType spriteType,
            SpriteGenerateOptions? options = null)
        {
            options ??= SpriteGenerateOptions.GetDefault();

            switch (spriteType)
            {
                case SpriteGenerateType.Auto:
                    if (node is INodeRect)
                    {
                        return GenerateRectSpriteFromSvg(node, svg, options.Value);
                    }
                    else
                    {
                        return GenerateSpriteFromSvg(node, svg, options.Value);
                    }

                case SpriteGenerateType.Path:
                    return GenerateSpriteFromSvg(node, svg, options.Value);

                case SpriteGenerateType.Rectangle:
                    return GenerateRectSpriteFromSvg(node, svg, options.Value);

                default:
                    throw new ArgumentOutOfRangeException(nameof(spriteType), spriteType, null);
            }
        }

        public static bool HasImageFill(SceneNode node)
        {
            // Ignore other node types, we can't import them anyway.
            if (node.type != NodeType.Rectangle && node.type != NodeType.Frame)
                return false;

            if (node is INodeFill nodeFill)
            {
                return nodeFill.fills.Any(x => x is ImagePaint);
            }

            return false;
        }

        private static Rect CalculateSvgViewBox(SceneNode node, bool isRectangle)
        {
            if (node is not INodeTransform transform)
            {
                Debug.LogWarning($"Node must implement {nameof(INodeTransform)}");
                return Rect.zero;
            }

            if (node is not INodeFill fill)
            {
                Debug.LogWarning($"Node must implement {nameof(INodeFill)}");
                return Rect.zero;
            }

            var strokeWeight = 0f;

            if (fill.HasStroke() && fill.strokeWeight.HasValue)
            {
                strokeWeight = fill.strokeWeight.Value;
            }

            var strokeAlign = fill.strokeAlign ?? StrokeAlign.Center;
            var strokePadding = strokeWeight;

            if (strokeAlign != StrokeAlign.Center)
            {
                strokePadding = isRectangle ? strokeWeight : strokeWeight * 2;
            }

            var strokeHalfPadding = strokePadding * 0.5f;

            return new Rect(
                x: -strokeHalfPadding,
                y: -strokeHalfPadding,
                width: transform.size.x + strokePadding,
                height: transform.size.y + strokePadding);
        }

        private static string GenerateSvgFromPath(SceneNode node, SceneNode overrideNode)
        {
            if (node is not VectorNode vectorNode || node is RectangleNode)
            {
                return GenerateSvgFromRect(node, overrideNode);
            }

            var width = vectorNode.size.x;
            var height = vectorNode.size.y;
            var viewBox = CalculateSvgViewBox(vectorNode, false);

            var svg = new StringBuilder();
            svg.Append($@"<svg id=""{node.id}"" ");
            svg.Append($@"width=""{viewBox.width}"" height=""{viewBox.height}"" ");
            svg.Append($@"viewBox=""{viewBox.x} {viewBox.y} {viewBox.width} {viewBox.height}"" ");
            svg.Append($@"fill=""none"" ");
            svg.AppendLine($@"xmlns=""http://www.w3.org/2000/svg"">");

            foreach (var geometry in vectorNode.fillGeometry)
            {
                var path = geometry.path;
                var windingRule = geometry.windingRule;
                PaintOverride paintOverride = null;

                if (vectorNode.fillOverrideTable != null && geometry.overrideId.HasValue)
                {
                    vectorNode.fillOverrideTable.TryGetValue(geometry.overrideId.Value, out paintOverride);
                }

                AppendSvgFillPathElement(
                    svg, node, path, new Vector2(width, height), overrideNode, paintOverride, windingRule);
            }

            foreach (var geometry in vectorNode.strokeGeometry)
            {
                AppendSvgStrokePathElement(svg, node, geometry.path, new Vector2(width, height), overrideNode);
            }

            svg.AppendLine("</svg>");

#if DEBUG_SVG_STRING
            Debug.Log($"{node}: {svg}");
#endif
            return svg.ToString();
        }

        private static Sprite GenerateSpriteFromSvg(SceneNode node, string svg, SpriteGenerateOptions options)
        {
            var sceneInfo = ImportSvg(svg);
            return CreateTexturedSprite(node, options, sceneInfo);
        }

        private static SVGParser.SceneInfo ImportSvg(string svg)
        {
            try
            {
                return SVGParser.ImportSVG(new StringReader(svg), ViewportOptions.PreserveViewport);
            }
            catch (Exception e) // SVGFormatException is internal class, so we need to catch its base class.
            {
                throw new SvgImportException(e.Message, svg, e);
            }
        }

        private static string GenerateSvgFromRect(SceneNode node, SceneNode overrideNode)
        {
            if (node is not INodeRect nodeRect)
                throw new ArgumentException("Specified node does not define a rectangle.", nameof(node));

            if (node is not INodeTransform nodeTransform)
                throw new ArgumentException("Specified node does not define a transform.", nameof(node));

            if (node is not INodeFill nodeFill)
                throw new ArgumentException("Specified node does not define a fill.", nameof(node));

            var width = nodeTransform.size.x;
            var height = nodeTransform.size.y;

            var strokeWeight = nodeFill.GetStrokeWeightOrDefault();
            var viewBox = CalculateSvgViewBox(node, true);
            var path = GetRectPath(nodeTransform, nodeRect);

            var svg = new StringBuilder();
            svg.Append($@"<svg id=""{node.id}"" ");
            svg.Append($@"width=""{viewBox.width}"" height=""{viewBox.height}"" ");
            svg.Append($@"viewBox=""{viewBox.x} {viewBox.y} {viewBox.width} {viewBox.height}"" ");
            svg.Append($@"fill=""none"" ");
            svg.AppendLine($@"xmlns=""http://www.w3.org/2000/svg"">");

            AppendSvgFillPathElement(svg, node, path, new Vector2(width, height), overrideNode);

            if (strokeWeight > 0)
            {
                AppendSvgStrokeRectElement(svg, node, path, new Vector2(width, height));
            }

            svg.AppendLine("</svg>");

#if DEBUG_SVG_STRING
            Debug.Log($"{node}: {svg}");
#endif
            return svg.ToString();
        }

        private static Sprite GenerateSpriteFromImage(FigmaFile file, SceneNode node, SpriteGenerateOptions options)
        {
            if (node is not INodeFill nodeFill)
                throw new ArgumentException("Specified node does not define a fill.", nameof(node));

            var imagePaint = nodeFill.fills.FirstOrDefault(x => x is ImagePaint) as ImagePaint;
            if (imagePaint == null)
                return null;

            if (!file.images.TryGetValue(imagePaint.imageRef, out var dataBase64))
                return null;

            var imageData = Convert.FromBase64String(dataBase64);

            var imageTexture = new Texture2D(1, 1);
            imageTexture.LoadImage(imageData, true);
            imageTexture.name = node.id;
            imageTexture.hideFlags = HideFlags.NotEditable;
            
            return CreateTexturedSprite(node, options, imageTexture);
        }

        private static Sprite GenerateRectSpriteFromSvg(SceneNode node, string svg, SpriteGenerateOptions options)
        {
            if (node is not INodeRect nodeRect)
                throw new ArgumentException("Specified node does not define a rectangle.", nameof(node));

            if (node is not INodeFill nodeFill)
                throw new ArgumentException("Specified node does not define a fill.", nameof(node));

            var strokeAlign = nodeFill.strokeAlign ?? StrokeAlign.Center;
            var strokeWidth = nodeFill.strokeWeight ?? 0;
            var strokePadding = strokeWidth;

            if (strokeAlign != StrokeAlign.Center)
            {
                strokePadding = strokeWidth * 2;
            }

            // Left, bottom, right and top.
            var borders = new Vector4(
                Mathf.Max(nodeRect.topLeftRadius, nodeRect.bottomLeftRadius, strokePadding),
                Mathf.Max(nodeRect.bottomLeftRadius, nodeRect.bottomRightRadius, strokePadding),
                Mathf.Max(nodeRect.topRightRadius, nodeRect.bottomRightRadius, strokePadding),
                Mathf.Max(nodeRect.topLeftRadius, nodeRect.topRightRadius, strokePadding)
            );
            
            var sceneInfo = ImportSvg(svg);
            return CreateTexturedSprite(node, options, sceneInfo, borders);
        }

        private static void AppendSvgGradientStops(StringBuilder svg, GradientPaint gradient)
        {
            foreach (var gradientStop in gradient.gradientStops)
            {
                svg.Append($@"<stop ");
                svg.Append($@"offset=""{gradientStop.position}"" ");
                svg.Append($@"stop-color=""{gradientStop.color.ToString("rgb-hex")}"" stop-opacity=""{gradientStop.color.a}"" ");
                svg.AppendLine("/>");
            }
        }

        private static void AppendSvgGradient(StringBuilder svg, SceneNode node, GradientPaint gradient, int index,
            Vector2 viewSize, bool isStroke)
        {
            var type = isStroke ? "stroke" : "fill";
            var gradientID = $"{type}{index}_{gradient.type.ToLowerInvariant()}_{NodeUtils.HyphenateNodeID(node.id)}";
            svg.AppendLine($@"{type}=""url(#{gradientID})"" />");
            
            if (gradient is LinearGradientPaint)
            {
                // Handles are normalized. So un-normalize them.
                var p1 = Vector2.Scale(gradient.gradientHandlePositions[0], viewSize);
                var p2 = Vector2.Scale(gradient.gradientHandlePositions[1], viewSize);

                svg.AppendLine(@"<defs>");
                svg.Append($@"<linearGradient ");
                svg.Append($@"id=""{gradientID}"" ");
                svg.Append($@"x1=""{p1.x}"" y1=""{p1.y}"" x2=""{p2.x}"" y2=""{p2.y}"" ");
                svg.Append($@"gradientUnits=""userSpaceOnUse"" ");
                svg.AppendLine($@">");

                AppendSvgGradientStops(svg, gradient);

                svg.AppendLine(@"</linearGradient>");
                svg.AppendLine(@"</defs>");
            }
            else if (gradient is RadialGradientPaint ||
                     gradient is DiamondGradientPaint ||
                     gradient is AngularGradientPaint)
            {
                // SVG supports only linear and radial gradients.
                // Handles are normalized. So un-normalize them.
                var p1 = Vector2.Scale(gradient.gradientHandlePositions[0], viewSize);
                var p2 = Vector2.Scale(gradient.gradientHandlePositions[1], viewSize);
                var p3 = Vector2.Scale(gradient.gradientHandlePositions[2], viewSize);
                var sx = Vector2.Distance(p1, p2);
                var sy = Vector2.Distance(p1, p3);

                var angle = Vector2.SignedAngle(Vector2.right, p2 - p1);

                svg.AppendLine(@"<defs>");
                svg.Append($@"<radialGradient ");
                svg.Append($@"id=""{gradientID}"" ");
                svg.Append($@"cx=""0"" cy=""0"" r=""1"" ");
                svg.Append($@"gradientUnits=""userSpaceOnUse"" ");
                svg.Append($@"gradientTransform=""translate({p1.x} {p1.y}) rotate({angle}) scale({sx} {sy})"" ");
                svg.AppendLine(">");

                AppendSvgGradientStops(svg, gradient);

                svg.AppendLine(@"</radialGradient>");
                svg.AppendLine(@"</defs>");
            }
            else
            {
                Debug.LogWarning($"Gradient type is not supported: {gradient.type}");
            }
        }

        private static void AppendSvgFillPathElement(StringBuilder svg, SceneNode node, string fillPath,
            Vector2 size, SceneNode overrideNode = null, 
            PaintOverride paintOverride = null, 
            WindingRule? windingRule = null)
        {
            List<Paint> fills = null;

            if (paintOverride != null)
            {
                fills = paintOverride.fills;
            }
            else
            {
                if (overrideNode is INodeFill fill)
                {
                    fills = fill.fills;
                }
                else
                {
                    fills = ((INodeFill)node)?.fills;
                }
            }

            if (fills == null)
                return;

            for (var i = 0; i < fills.Count; i++)
            {
                var fill = fills[i];

                if (!fill.visible)
                    continue;

                svg.Append($@"<path d=""{fillPath}"" ");
                svg.Append($@"fill-opacity=""{fill.opacity}"" ");

                if (windingRule.HasValue && windingRule.Value != WindingRule.None)
                {
                    var windingRuleString = GetWindingRuleString(windingRule.Value);
                    svg.Append($@"fill-rule=""{windingRuleString}"" ");
                    svg.Append($@"clip-rule=""{windingRuleString}"" ");
                }
                
                if (fill is SolidPaint solid)
                {
                    AppendSvgSolid(svg, node, solid, size, i, false, windingRule);
                }
                else if (fill is GradientPaint gradient)
                {
                    AppendSvgGradient(svg, node, gradient, i, size, false);
                }
                else
                {
                    svg.AppendLine("/>");
                }
            }
        }

        private static void AppendSvgSolidAsGradient(StringBuilder svg, SolidPaint solid, string gradientID,
            Vector2 viewSize)
        {
            // Handles are normalized. So un-normalize them.
            var p1 = Vector2.Scale(new Vector2(0, 0), viewSize);
            var p2 = Vector2.Scale(new Vector2(1, 1), viewSize);

            svg.AppendLine(@"<defs>");
            svg.Append($@"<linearGradient ");
            svg.Append($@"id=""{gradientID}"" ");
            svg.Append($@"x1=""{p1.x}"" y1=""{p1.y}"" x2=""{p2.x}"" y2=""{p2.y}"" ");
            svg.Append($@"gradientUnits=""userSpaceOnUse"" ");
            svg.AppendLine($@">");
            
            svg.Append($@"<stop ");
            svg.Append($@"offset=""0"" ");
            svg.Append($@"stop-color=""{solid.color.ToString("rgb-hex")}"" stop-opacity=""{solid.opacity}"" ");
            svg.AppendLine("/>");
            
            svg.Append($@"<stop ");
            svg.Append($@"offset=""1"" ");
            svg.Append($@"stop-color=""{solid.color.ToString("rgb-hex")}"" stop-opacity=""{solid.opacity}"" ");
            svg.AppendLine("/>");
            
            svg.AppendLine(@"</linearGradient>");
            svg.AppendLine(@"</defs>");
        }

        private static void AppendSvgStrokePathElement(StringBuilder svg, SceneNode node, string strokePath,
            Vector2 size, SceneNode overrideNode = null)
        {
            List<Paint> strokes = null;

            if (overrideNode is INodeFill fill)
            {
                strokes = fill.strokes;
            }
            else
            {
                strokes = ((INodeFill)node)?.strokes;
            }

            if (strokes == null)
                return;

            var nodeFill = (INodeFill)node;
            Debug.Assert(nodeFill != null);

            for (var i = 0; i < strokes.Count; i++)
            {
                var stroke = strokes[i];

                if (!stroke.visible)
                    continue;

                svg.Append($@"<path d=""{strokePath}"" ");

                if (stroke is SolidPaint solid)
                {
                    AppendSvgSolid(svg, node, solid, size, i, true);
                }
                else if (stroke is GradientPaint gradient)
                {
                    AppendSvgGradient(svg, node, gradient, i, size, true);
                }
                else
                {
                    svg.AppendLine("/>");
                }
            }
        }

        private static void AppendSvgSolid(StringBuilder svg, SceneNode node, SolidPaint solid, Vector2 size, 
            int index, bool isStroke, WindingRule? windingRule = null)
        {
            if (isStroke)
            {
                // Gradient stroke is not supported by Vector Graphics package.
                svg.AppendLine($@"stroke=""{solid.color.ToString("rgb-hex")}"" />");
                return;
            }
            
            // Using solid fill color could not be generated in Unity Cloud Build.
            // So we generate solid fills using gradient.
            // https://forum.unity.com/threads/vector-graphics-preview-package.529845/page-27#post-8999671
            //svg.AppendLine($@"fill=""{solid.color.ToString("rgb-hex")}"" />");

            if (windingRule.HasValue && windingRule.Value == WindingRule.EvenOdd)
            {
                // Using gradient while winding rule is EvenOdd fails to render it correctly.
                // So we have to use it as normal way.
                svg.AppendLine($@"fill=""{solid.color.ToString("rgb-hex")}"" />");
            }
            else
            {
                var gradientID = $"fill_{index}_solid_{NodeUtils.HyphenateNodeID(node.id)}";
                svg.AppendLine($@"fill=""url(#{gradientID})"" />");

                AppendSvgSolidAsGradient(svg, solid, gradientID, size);    
            }
        }

        private static void AppendSvgStrokeRectElement(StringBuilder svg, SceneNode node, string strokePath, 
            Vector2 size)
        {
            var nodeFill = (INodeFill)node;
            Debug.Assert(nodeFill != null);

            var strokeAlign = SvgHelpers.GetSvgValue(nodeFill.strokeAlign);
            var strokeWidth = nodeFill.strokeWeight ?? 0;
            for (var i = 0; i < nodeFill.strokes.Count; i++)
            {
                var stroke = nodeFill.strokes[i];

                if (!stroke.visible)
                    continue;

                svg.Append($@"<path d=""{strokePath}"" ");
                svg.Append($@"fill=""none"" ");
                svg.Append($@"stroke-width=""{strokeWidth}"" ");
                svg.Append($@"stroke-opacity=""{stroke.opacity}"" ");
                svg.Append($@"stroke-alignment=""{strokeAlign}"" ");

                if (nodeFill.strokeDashes != null)
                {
                    svg.Append($@"stroke-dasharray=""{string.Join(' ', nodeFill.strokeDashes)}"" ");
                }

                if (stroke is SolidPaint solid)
                {
                    AppendSvgSolid(svg, node, solid, size, i, true);
                }
                else if (stroke is GradientPaint gradient)
                {
                    AppendSvgGradient(svg, node, gradient, i, size, true);
                }
                else
                {
                    svg.AppendLine("/>");
                }
            }
        }


        private static Sprite CreateTexturedSprite(SceneNode node, SpriteGenerateOptions options,
            SVGParser.SceneInfo sceneInfo, Vector4? borders = null)
        {
            var geometries =
                VectorUtils.TessellateScene(sceneInfo.Scene, options.tessellationOptions, sceneInfo.NodeOpacity);

            var sprite = VectorUtils.BuildSprite(geometries, sceneInfo.SceneViewport,
                options.pixelsPerUnit, VectorUtils.Alignment.Center,
                Vector2.zero, options.gradientResolution, true);

            if (sprite == null)
                return null;

            var spriteWidth = sprite.rect.width * options.scaleFactor;
            var spriteHeight = sprite.rect.height * options.scaleFactor;

            var sizeRatio = 1f;

            if (spriteWidth > options.maxTextureSize || spriteHeight > options.maxTextureSize)
            {
                var widthRatio = options.maxTextureSize / spriteWidth;
                var heightRatio = options.maxTextureSize / spriteHeight;
                sizeRatio = Mathf.Min(widthRatio, heightRatio);
            }
            else if (spriteWidth < options.minTextureSize && spriteHeight < options.minTextureSize)
            {
                var widthRatio = options.minTextureSize / spriteWidth;
                var heightRatio = options.minTextureSize / spriteHeight;
                sizeRatio = Mathf.Min(widthRatio, heightRatio);
            }

            var widthScaled = spriteWidth * sizeRatio;
            var heightScaled = spriteHeight * sizeRatio;

            // Use sprite's original dimensions to preserve its aspect ratio.
            if (widthScaled < 1 || heightScaled < 1)
            {
                widthScaled = spriteWidth;
                heightScaled = spriteHeight;
            }

            var textureWidth = Mathf.RoundToInt(Mathf.Max(widthScaled, 1f));
            var textureHeight = Mathf.RoundToInt(Mathf.Max(heightScaled, 1f));

            var expandEdges = options.filterMode != FilterMode.Point || options.sampleCount > 1;
            var material = GetSpriteMaterial();
            var texture =
                VectorUtils.RenderSpriteToTexture2D(
                    sprite, textureWidth, textureHeight, material, options.sampleCount, expandEdges);

            if (texture != null)
            {
                texture.name = node.id;
                texture.hideFlags = HideFlags.NotEditable;
                texture.filterMode = options.filterMode;
                texture.wrapMode = options.wrapMode;
            }

            Object.DestroyImmediate(sprite);

#if UNITY_EDITOR
            if (!UnityEditor.EditorUtility.IsPersistent(material))
            {
#endif
                Object.DestroyImmediate(material);
#if UNITY_EDITOR
            }
#endif

            if (texture == null)
                return null;

            return CreateTexturedSprite(node, options, texture, sizeRatio, borders);
        }

        private static Sprite CreateTexturedSprite(SceneNode node, SpriteGenerateOptions options,
            Texture2D texture, float scale = 1f, Vector4? borders = null)
        {
            var spriteRect = new Rect(0, 0, texture.width, texture.height);
            var spritePivot = spriteRect.center;

            var pixelsPerUnit = options.pixelsPerUnit * scale;

            Sprite spriteWithTexture = null;
            if (borders.HasValue)
            {
                borders *= scale * options.scaleFactor;
                spriteWithTexture = Sprite.Create(
                    texture, spriteRect, spritePivot, pixelsPerUnit, 0, SpriteMeshType.FullRect, borders.Value);
            }
            else
            {
                spriteWithTexture = Sprite.Create(texture, spriteRect, spritePivot, pixelsPerUnit, 0);
            }

            spriteWithTexture.name = node.id;
            spriteWithTexture.hideFlags = HideFlags.NotEditable;

            return spriteWithTexture;
        }

        private static string GetRectPath(INodeTransform nodeTransform, INodeRect nodeRect)
        {
            var rect = new Rect(0, 0, nodeTransform.size.x, nodeTransform.size.y);
            var radiusTL = Vector2.one * nodeRect.topLeftRadius;
            var radiusTR = Vector2.one * nodeRect.topRightRadius;
            var radiusBR = Vector2.one * nodeRect.bottomRightRadius;
            var radiusBL = Vector2.one * nodeRect.bottomLeftRadius;
            var contour = VectorUtils.BuildRectangleContour(rect, radiusTL, radiusTR, radiusBR, radiusBL);
            var segments = contour.Segments;

            Debug.Assert(segments.Length > 1);

            var svgString = new StringBuilder();
            svgString.Append($@"M {segments[0].P0.x} {segments[0].P0.y} ");
            for (var i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                var segmentNext = i < segments.Length - 1 ? segments[i + 1] : segments[0];
                var control1 = segment.P1;
                var control2 = segment.P2;
                var end = segmentNext.P0;

                svgString.Append($@"C {control1.x} {control1.y} {control2.x} {control2.y} {end.x} {end.y} ");
            }

            svgString.Append("Z");
            return svgString.ToString();
        }
        
        private static string GetWindingRuleString(WindingRule windingRule)
        {
            switch (windingRule)
            {
                case WindingRule.None:
                    return "none";
                case WindingRule.NonZero:
                    return "nonzero";
                case WindingRule.EvenOdd:
                    return "evenodd";
                default:
                    throw new ArgumentOutOfRangeException(nameof(windingRule), windingRule, null);
            }
        }

        private const string SpriteMaterialPath =
            "Packages/com.unity.vectorgraphics/Runtime/Materials/Unlit_VectorGradient.mat";

        private const string SpriteShaderName = "Unlit/VectorGradient";

        private static Material GetSpriteMaterial()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                return new Material(Shader.Find(SpriteShaderName));
#if UNITY_EDITOR
            }
#endif

#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(SpriteMaterialPath);
#endif
        }
    }
}