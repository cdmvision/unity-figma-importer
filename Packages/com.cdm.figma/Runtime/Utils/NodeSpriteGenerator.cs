using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Cdm.Figma.Utils
{
    public enum SpriteGenerateType
    {
        Auto,
        Path,
        Rectangle
    }

    public class NodeSpriteGenerator
    {
        public class SpriteOptions
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
            public float svgPixelsPerUnit { get; set; } = 100f;
            public float pixelsPerUnit { get; set; } = 100f;
        }

        /// <summary>
        /// Generates a sprite from the scene node.
        /// </summary>
        public static Sprite GenerateSprite(SceneNode node, SpriteGenerateType spriteType,
            SpriteOptions options = null)
        {
            var svg = GenerateSpriteSvg(node, spriteType);
            //Debug.Log($"{node}: {svg}");
            return GenerateSprite(node, svg, spriteType, options);
        }
        
        /// <summary>
        /// Generates SVG string from the scene node.
        /// </summary>
        public static string GenerateSpriteSvg(SceneNode node, SpriteGenerateType spriteType)
        {
            switch (spriteType)
            {
                case SpriteGenerateType.Auto:
                    if (node is INodeRect)
                    {
                        return GenerateSvgFromRect(node);
                    }
                    else
                    {
                        return GenerateSvgFromPath(node);
                    }

                case SpriteGenerateType.Path:
                    return GenerateSvgFromPath(node);

                case SpriteGenerateType.Rectangle:
                    return GenerateSvgFromRect(node);
                default:
                    throw new ArgumentOutOfRangeException(nameof(spriteType), spriteType, null);
            }
        }

        /// <summary>
        /// Generates a sprite from the scene node and the SVG string given.
        /// </summary>
        public static Sprite GenerateSprite(SceneNode node, string svg, SpriteGenerateType spriteType,
            SpriteOptions options = null)
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

        private static string GenerateSvgFromPath(SceneNode node)
        {
            if (node is not VectorNode vectorNode)
                throw new ArgumentException($"Node type must be {NodeType.Vector}.", nameof(node));

            var width = vectorNode.size.x;
            var height = vectorNode.size.y;

            var svgString = new StringBuilder();
            svgString.AppendLine($@"<svg width=""{width}"" height=""{height}"" xmlns=""http://www.w3.org/2000/svg"">");

            foreach (var geometry in vectorNode.fillGeometry)
            {
                var path = geometry.path;
                var windingRule = geometry.windingRule;
                AppendSvgPathElement(svgString, node, path, new Vector2(width, height), windingRule);
            }

            svgString.AppendLine("</svg>");
            return svgString.ToString();
        }

        private static Sprite GenerateSpriteFromSvg(SceneNode node, string svg, SpriteOptions options)
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
            var strokeWeight = nodeFill.strokeWeight ?? 0;
            var strokeHalfWeight = strokeWeight * 0.5f;
            var viewBox = new Rect(-strokeHalfWeight, -strokeHalfWeight, width + strokeWeight, height + strokeWeight);

            var path = GetRectPath(nodeTransform, nodeRect);
            var svg = new StringBuilder();

            svg.Append($@"<svg id=""{node.id}"" ");
            svg.Append($@"width=""{width}"" height=""{height}"" ");
            svg.Append($@"viewBox=""{viewBox.x} {viewBox.y} {viewBox.width} {viewBox.height}"" ");
            svg.Append($@"fill=""none"" ");
            svg.AppendLine($@"xmlns=""http://www.w3.org/2000/svg"">");

            AppendSvgPathElement(svg, node, path, new Vector2(width, height));

            svg.AppendLine("</svg>");

            //Debug.Log($"{node}: {svg}");
            return svg.ToString();
        }

        private static Sprite GenerateRectSpriteFromSvg(SceneNode node, string svg, SpriteOptions options)
        {
            if (node is not INodeRect nodeRect)
                throw new ArgumentException("Specified node does not define a rectangle.", nameof(node));

            if (node is not INodeTransform nodeTransform)
                throw new ArgumentException("Specified node does not define a transform.", nameof(node));

            if (node is not INodeFill nodeFill)
                throw new ArgumentException("Specified node does not define a fill.", nameof(node));

            // Left, bottom, right and top.
            var strokeWidth = nodeFill.strokeWeight ?? 0;
            var strokePadding = strokeWidth * 2 + 4;
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

        private static void AppendSvgPathElement(StringBuilder svg, SceneNode node, string path, Vector2 size,
            string windingRule = null)
        {
            var nodeFill = (INodeFill)node;
            Debug.Assert(nodeFill != null);

            for (var i = 0; i < nodeFill.fills.Count; i++)
            {
                var fill = nodeFill.fills[i];

                if (!fill.visible)
                    continue;

                svg.Append($@"<path d=""{path}"" ");
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

            var strokeAlign = SvgHelpers.GetSvgValue(nodeFill.strokeAlign);
            var strokeWidth = nodeFill.strokeWeight ?? 0;
            for (var i = 0; i < nodeFill.strokes.Count; i++)
            {
                var stroke = nodeFill.strokes[i];

                if (!stroke.visible)
                    continue;

                svg.Append($@"<path d=""{path}"" ");
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

        private static Sprite CreateTexturedSprite(SceneNode node, SpriteOptions options,
            SVGParser.SceneInfo sceneInfo, Vector4? borders = null)
        {
            var geometries = VectorUtils.TessellateScene(sceneInfo.Scene, options.tessellationOptions, sceneInfo.NodeOpacity);
            
            var sprite = VectorUtils.BuildSprite(geometries, sceneInfo.SceneViewport, options.svgPixelsPerUnit, 
                VectorUtils.Alignment.TopLeft, Vector2.zero, options.gradientResolution, true);
            
            if (sprite == null)
                return null;

            var widthRatio = options.textureSize / sprite.rect.width;
            var heightRatio = options.textureSize / sprite.rect.height;

            var ratio = Mathf.Min(widthRatio, heightRatio);

            var width = (int)(sprite.rect.width * ratio);
            var height = (int)(sprite.rect.height * ratio);


            var expandEdges = options.filterMode != FilterMode.Point || options.sampleCount > 1;
            var material = new Material(Shader.Find("Unlit/VectorGradient"));
            var texture =
                VectorUtils.RenderSpriteToTexture2D(sprite, width, height, material, options.sampleCount, expandEdges);

            if (texture != null)
            {
                texture.filterMode = options.filterMode;
                texture.wrapMode = options.wrapMode;
            }

            Object.DestroyImmediate(sprite);
            Object.DestroyImmediate(material);

            if (texture == null)
                return null;

            var spriteRect = new Rect(0, 0, texture.width, texture.height);
            var spritePivot = spriteRect.center;

            var pixelsPerUnity = options.pixelsPerUnit * ratio;

            Sprite spriteWithTexture = null;
            if (borders.HasValue)
            {
                borders *= ratio;
                spriteWithTexture = Sprite.Create(
                    texture, spriteRect, spritePivot, pixelsPerUnity, 0, SpriteMeshType.FullRect, borders.Value);
            }
            else
            {
                spriteWithTexture = Sprite.Create(texture, spriteRect, spritePivot, pixelsPerUnity, 0);
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

            return svgString.ToString();
        }
    }
}