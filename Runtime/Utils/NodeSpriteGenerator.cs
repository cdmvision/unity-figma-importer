using System;
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

    public class SpriteGenerateOptions
    {
        public VectorUtils.TessellationOptions tessellationOptions { get; set; } =
            new VectorUtils.TessellationOptions()
            {
                StepDistance = 2.0f,
                MaxCordDeviation = 0.5f,
                MaxTanAngleDeviation = 0.1f,
                SamplingStepSize = 0.01f
            };

        public FilterMode filterMode { get; set; } = FilterMode.Bilinear;
        public TextureWrapMode wrapMode { get; set; } = TextureWrapMode.Clamp;
        public int textureSize { get; set; } = 256;
        public int sampleCount { get; set; } = 4;
        public ushort gradientResolution { get; set; } = 128;
        public float pixelsPerUnit { get; set; } = 100f;
    }
    
    public class NodeSpriteGenerator
    {
        /// <summary>
        /// Generates a sprite from the scene node.
        /// </summary>
        public static Sprite GenerateSprite(FigmaFile file, SceneNode node, SpriteGenerateType spriteType,
            SpriteGenerateOptions options = null)
        {
            if (HasImageFill(node))
            {
                var imageSprite = GenerateSpriteFromImage(file, node, options);
                if (imageSprite != null)
                    return imageSprite;
            }
            
            var svg = GenerateSpriteSvg(node);
            var sprite = GenerateSprite(node, svg, spriteType, options);
            //Debug.Log($"{node}, [Sprite Generated:{(sprite != null ? "YES": "NO")}]: {svg}");
            
            return sprite;
        }

        /// <summary>
        /// Generates SVG string from the scene node.
        /// </summary>
        public static string GenerateSpriteSvg(SceneNode node)
        {
            return GenerateSvgFromPath(node);
        }

        /// <summary>
        /// Generates a sprite from the scene node and the SVG string given.
        /// </summary>
        public static Sprite GenerateSprite(SceneNode node, string svg, SpriteGenerateType spriteType,
            SpriteGenerateOptions options = null)
        {
            switch (spriteType)
            {
                case SpriteGenerateType.Auto:
                    if (node is INodeRect)
                    {
                        return GenerateRectSpriteFromSvg(node, svg, options);
                    }
                    else
                    {
                        return GenerateSpriteFromSvg(node, svg, options);
                    }

                case SpriteGenerateType.Path:
                    return GenerateSpriteFromSvg(node, svg, options);

                case SpriteGenerateType.Rectangle:
                    return GenerateRectSpriteFromSvg(node, svg, options);

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

        private static Rect CalculateSvgViewBox(SceneNode node)
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
                strokePadding = strokeWeight * 2;
            }

            var strokeHalfPadding = strokePadding * 0.5f;

            return new Rect(
                x: -strokeHalfPadding,
                y: -strokeHalfPadding,
                width: transform.size.x + strokePadding,
                height: transform.size.y + strokePadding);
        }

        private static string GenerateSvgFromPath(SceneNode node)
        {
            if (node is not VectorNode vectorNode)
            {
                return GenerateSvgFromRect(node);
            }

            var width = vectorNode.size.x;
            var height = vectorNode.size.y;
            var viewBox = CalculateSvgViewBox(vectorNode);

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

                AppendSvgFillPathElement(svg, node, path, new Vector2(width, height), paintOverride, windingRule);
            }

            foreach (var geometry in vectorNode.strokeGeometry)
            {
                var path = geometry.path;
                var windingRule = geometry.windingRule;
                AppendSvgStrokePathElement(svg, node, path, new Vector2(width, height), windingRule);
            }

            svg.AppendLine("</svg>");

            //Debug.Log($"{node}: {svg}");
            return svg.ToString();
        }

        private static Sprite GenerateSpriteFromSvg(SceneNode node, string svg, SpriteGenerateOptions options)
        {
            var sceneInfo = SVGParser.ImportSVG(new StringReader(svg), ViewportOptions.PreserveViewport);
            return CreateTexturedSprite(node, options, sceneInfo);
        }

        private static string GenerateSvgFromRect(SceneNode node)
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
            var viewBox = CalculateSvgViewBox(node);
            
            var fillPath = GetRectPath(nodeTransform, nodeRect);
            var strokePath = GetRectPath(nodeTransform, nodeRect);
            
            var svg = new StringBuilder();
            svg.Append($@"<svg id=""{node.id}"" ");
            svg.Append($@"width=""{viewBox.width}"" height=""{viewBox.height}"" ");
            svg.Append($@"viewBox=""{viewBox.x} {viewBox.y} {viewBox.width} {viewBox.height}"" ");
            svg.Append($@"fill=""none"" ");
            svg.AppendLine($@"xmlns=""http://www.w3.org/2000/svg"">");

            AppendSvgFillPathElement(svg, node, fillPath, new Vector2(width, height));

            if (strokeWeight > 0)
            {
                AppendSvgStrokeRectElement(svg, node, strokePath, new Vector2(width, height));
            }

            svg.AppendLine("</svg>");

            //Debug.Log($"{node}: {svg}");
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
            
            var sceneInfo = SVGParser.ImportSVG(new StringReader(svg), ViewportOptions.PreserveViewport);
            return CreateTexturedSprite(node, options, sceneInfo, borders);
        }

        private static void AppendSvgGradientStops(StringBuilder svg, GradientPaint gradient)
        {
            foreach (var gradientStop in gradient.gradientStops)
            {
                svg.Append($@"<stop ");
                svg.Append($@"offset=""{gradientStop.position}"" ");
                svg.Append($@"stop-color=""{gradientStop.color.ToString("rgb-hex")}"" ");
                svg.AppendLine("/>");
            }
        }

        private static void AppendSvgGradient(StringBuilder svg, GradientPaint gradient, string gradientID,
            Vector2 viewSize)
        {
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
            Vector2 size, PaintOverride paintOverride = null, string windingRule = null)
        {
            var fills = paintOverride != null ? paintOverride.fills : ((INodeFill)node)?.fills;
            if (fills == null)
                return;
            
            for (var i = 0; i < fills.Count; i++)
            {
                var fill = fills[i];

                if (!fill.visible)
                    continue;

                svg.Append($@"<path d=""{fillPath}"" ");
                svg.Append($@"fill-opacity=""{fill.opacity}"" ");

                if (!string.IsNullOrEmpty(windingRule))
                {
                    svg.Append($@"fill-rule=""{windingRule.ToLowerInvariant()}"" ");
                    svg.Append($@"clip-rule=""{windingRule.ToLowerInvariant()}"" ");
                }

                if (fill is SolidPaint solid)
                {
                    svg.AppendLine($@"fill=""{solid.color.ToString("rgb-hex")}"" />");
                }
                else if (fill is GradientPaint gradient)
                {
                    var gradientID = $"f{i}_{gradient.type.ToLowerInvariant()}_{NodeUtils.HyphenateNodeID(node.id)}";

                    svg.AppendLine($@"fill=""url(#{gradientID})"" />");

                    AppendSvgGradient(svg, gradient, gradientID, size);
                }
                else
                {
                    svg.AppendLine("/>");
                }
            }
        }
        
        private static void AppendSvgStrokePathElement(StringBuilder svg, SceneNode node, string strokePath, 
            Vector2 size, string windingRule = null)
        {
            var nodeFill = (INodeFill)node;
            Debug.Assert(nodeFill != null);
            
            for (var i = 0; i < nodeFill.strokes.Count; i++)
            {
                var stroke = nodeFill.strokes[i];

                if (!stroke.visible)
                    continue;

                svg.Append($@"<path d=""{strokePath}"" ");
                
                if (stroke is SolidPaint solid)
                {
                    svg.AppendLine($@"fill=""{solid.color.ToString("rgb-hex")}"" />");
                }
                else if (stroke is GradientPaint gradient)
                {
                    var gradientID = $"s{i}_{gradient.type.ToLowerInvariant()}_{NodeUtils.HyphenateNodeID(node.id)}";

                    svg.AppendLine($@"fill=""url(#{gradientID})"" />");

                    AppendSvgGradient(svg, gradient, gradientID, size);
                }
                else
                {
                    svg.AppendLine("/>");
                }
            }
        }

        private static void AppendSvgStrokeRectElement(StringBuilder svg, SceneNode node, string strokePath,
            Vector2 size, string windingRule = null)
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
                    svg.AppendLine($@"stroke=""{solid.color.ToString("rgb-hex")}"" />");
                }
                else if (stroke is GradientPaint gradient)
                {
                    var gradientID = $"s{i}_{gradient.type.ToLowerInvariant()}_{NodeUtils.HyphenateNodeID(node.id)}";

                    svg.AppendLine($@"stroke=""url(#{gradientID})"" />");

                    AppendSvgGradient(svg, gradient, gradientID, size);
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

            var widthRatio = options.textureSize / sprite.rect.width;
            var heightRatio = options.textureSize / sprite.rect.height;

            var ratio = Mathf.Min(widthRatio, heightRatio);
            var widthScaled = sprite.rect.width * ratio;
            var heightScaled = sprite.rect.height * ratio;

            // Use sprite's original dimensions to preserve its aspect ratio.
            if (widthScaled < 1 || heightScaled < 1)
            {
                widthScaled = sprite.rect.width;
                heightScaled = sprite.rect.height;
            }

            var textureWidth = Mathf.RoundToInt(Mathf.Max(widthScaled, 1f));
            var textureHeight = Mathf.RoundToInt(Mathf.Max(heightScaled, 1f));

            var expandEdges = options.filterMode != FilterMode.Point || options.sampleCount > 1;
            var material = new Material(Shader.Find("Unlit/VectorGradient"));
            var texture =
                VectorUtils.RenderSpriteToTexture2D(
                    sprite, textureWidth, textureHeight, material, options.sampleCount, expandEdges);

            if (texture != null)
            {
                texture.filterMode = options.filterMode;
                texture.wrapMode = options.wrapMode;
            }

            Object.DestroyImmediate(sprite);
            Object.DestroyImmediate(material);

            if (texture == null)
                return null;

            return CreateTexturedSprite(node, options, texture, ratio, borders);
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
                borders *= scale;
                spriteWithTexture = Sprite.Create(
                    texture, spriteRect, spritePivot, pixelsPerUnit, 0, SpriteMeshType.FullRect, borders.Value);
            }
            else
            {
                spriteWithTexture = Sprite.Create(texture, spriteRect, spritePivot, pixelsPerUnit, 0);
            }

            spriteWithTexture.name = node.id;

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
    }
}