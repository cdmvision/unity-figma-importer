using System;
using System.Collections.Generic;
using System.Globalization;
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

    public struct GeneratedSprite
    {
        public Sprite sprite { get; set; }
        public bool isNew { get; set; }

        public GeneratedSprite(Sprite sprite, bool isNew)
        {
            this.sprite = sprite;
            this.isNew = isNew;
        }
    }
    
    public class NodeSpriteGenerator
    {
        private readonly Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

        /// <summary>
        /// Gets the sprite immediately if exist; otherwise sprite is generated and added to the sprite database
        /// for later use.
        /// </summary>
        public GeneratedSprite GetOrGenerateSprite(FigmaFile file, SceneNode node, SpriteGenerateType spriteType, 
            SpriteGenerateOptions? options = null)
        {
            return GenerateSpriteInternal(file, node, spriteType, options, _sprites);
        }
        
        /// <summary>
        /// Generates a sprite from the scene node.
        /// </summary>
        public static Sprite GenerateSprite(FigmaFile file, SceneNode node, SpriteGenerateType spriteType,
            SpriteGenerateOptions? options = null)
        {
            return GenerateSpriteInternal(file, node, spriteType, options, null).sprite;
        }

        private static GeneratedSprite GenerateSpriteInternal(FigmaFile file, SceneNode node, SpriteGenerateType spriteType,
            SpriteGenerateOptions? options, Dictionary<string, Sprite> spriteDatabase)
        {
            options ??= SpriteGenerateOptions.GetDefault(); 
            
            if (HasImageFill(node))
            {
                var imageSprite = GenerateSpriteFromImage(file, node, options.Value);
                if (imageSprite != null)
                    return new GeneratedSprite(imageSprite, true);
            }

            var svg = GenerateSvgFromPath(node, options.Value.overrideNode, options.Value);
            
#if FIGMA_PRINT_SVG_STRING
            Debug.Log($"{svg.node}: {svg.content}");
#endif

            var generatedSprite = new GeneratedSprite();
            
            if (spriteDatabase != null)
            {
                // Comparing svg string is faster than calculating hashes and comparing the them.
                // So just use svg string for the key.
                
                if (spriteDatabase.TryGetValue(svg.content, out var sprite))
                {
                    generatedSprite = new GeneratedSprite(sprite, false);
                }
                else
                {
                    sprite = GenerateSprite(node, svg.content, spriteType, svg.dontResize, options);

                    if (sprite != null)
                    {
                        generatedSprite = new GeneratedSprite(sprite, true);
                        spriteDatabase.Add(svg.content, sprite);
                    }
                }
            }
            else
            {
                var sprite = GenerateSprite(node, svg.content, spriteType, svg.dontResize, options);
                generatedSprite = new GeneratedSprite(sprite, true);
            }
            
            //Debug.Log($"{node}, [Sprite Generated:{(sprite != null ? "YES": "NO")}]: {svg}");

            return generatedSprite;
        }
        
        /// <summary>
        /// Generates SVG string from the scene node.
        /// </summary>
        public static string GenerateSpriteSvg(SceneNode node, SceneNode overrideNode, SpriteGenerateOptions options)
        {
            return GenerateSvgFromPath(node, overrideNode, options).content;
        }

        /// <summary>
        /// Generates a sprite from the scene node and the SVG string given.
        /// </summary>
        public static Sprite GenerateSprite(SceneNode node, string svg, SpriteGenerateType spriteType,
            bool dontResize, SpriteGenerateOptions? options = null)
        {
            options ??= SpriteGenerateOptions.GetDefault();

            switch (spriteType)
            {
                case SpriteGenerateType.Auto:
                    if (node is INodeRect)
                    {
                        return GenerateRectSpriteFromSvg(node, svg, dontResize, options.Value);
                    }
                    else
                    {
                        return GenerateSpriteFromSvg(node, svg, options.Value);
                    }

                case SpriteGenerateType.Path:
                    return GenerateSpriteFromSvg(node, svg, options.Value);

                case SpriteGenerateType.Rectangle:
                    return GenerateRectSpriteFromSvg(node, svg, dontResize, options.Value);

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

        private static Rect CalculateSvgViewBox(SceneNode node, float width, float height, bool isRectangle, float resolutionScale = 1.0f)
        {
            if (node is not INodeFill fill)
            {
                Debug.LogWarning($"Node must implement {nameof(INodeFill)}");
                return Rect.zero;
            }

            var strokeWeight = 0f;

            if (fill.HasStroke() && fill.strokeWeight.HasValue)
            {
                strokeWeight = fill.strokeWeight.Value * resolutionScale;
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
                width: width + strokePadding,
                height: height + strokePadding);
        }

        private static SvgString GenerateSvgFromPath(SceneNode node, SceneNode overrideNode, SpriteGenerateOptions options)
        {
            if (node is not VectorNode vectorNode || node is RectangleNode)
            {
                return GenerateSvgFromRect(node, overrideNode, options);
            }

            var width = vectorNode.size.x;
            var height = vectorNode.size.y;
            var viewBox = CalculateSvgViewBox(vectorNode, width, height, false);

            var svg = new StringBuilder();
            svg.Append($@"<svg ");
            AppendSvgSizeAndViewBox(svg, viewBox);
            svg.Append($@"fill=""none"" ");
            svg.AppendLine($@"xmlns=""http://www.w3.org/2000/svg"">");

            for (var geometryIndex = 0; geometryIndex < vectorNode.fillGeometry.Length; geometryIndex++)
            {
                var geometry = vectorNode.fillGeometry[geometryIndex];
                var path = geometry.path;
                var windingRule = geometry.windingRule;
                var size = new Vector2(width, height);
                PaintOverride paintOverride = null;

                if (vectorNode.fillOverrideTable != null && geometry.overrideId.HasValue)
                {
                    vectorNode.fillOverrideTable.TryGetValue(geometry.overrideId.Value, out paintOverride);
                }

                var fills = GetSvgFills(node, overrideNode, paintOverride);
                AppendSvgFillPathElement(svg, node, path, geometryIndex, size, fills, windingRule);
            }


            for (var geometryIndex = 0; geometryIndex < vectorNode.strokeGeometry.Length; geometryIndex++)
            {
                var geometry = vectorNode.strokeGeometry[geometryIndex];
                var size = new Vector2(width, height);

                AppendSvgStrokePathElement(svg, node, geometry.path, geometryIndex, size, overrideNode);
            }

            svg.AppendLine("</svg>");
            return new SvgString(node, svg.ToString());
        }

        private static Sprite GenerateSpriteFromSvg(SceneNode node, string svg, SpriteGenerateOptions options)
        {
            var sceneInfo = ImportSvg(svg);
            return CreateSprite(node, options, sceneInfo);
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

        private static SvgString GenerateSvgFromRect(SceneNode node, SceneNode overrideNode, 
            SpriteGenerateOptions options)
        {
            if (node is not INodeRect nodeRect)
                throw new ArgumentException("Specified node does not define a rectangle.", nameof(node));

            if (node is not INodeTransform nodeTransform)
                throw new ArgumentException("Specified node does not define a transform.", nameof(node));

            if (node is not INodeFill nodeFill)
                throw new ArgumentException("Specified node does not define a fill.", nameof(node));

            var width = nodeTransform.size.x;
            var height = nodeTransform.size.y;
            
            var fills = GetSvgFills(node, overrideNode, null);
            
            var dontResize = false;
            var isGradientExist = fills.Any(x => x is GradientPaint);

            if (!isGradientExist)
            {
                width = options.rectTextureSize + (nodeRect.topLeftRadius + nodeRect.topRightRadius) * options.rectResolutionScale;
                height = options.rectTextureSize + (nodeRect.bottomLeftRadius + nodeRect.bottomRightRadius) * options.rectResolutionScale;
                dontResize = true;
            }

            var strokeWeight = nodeFill.GetStrokeWeightOrDefault() * options.rectResolutionScale;
            var viewBox = CalculateSvgViewBox(node, width, height, true, options.rectResolutionScale);
            var path = GetRectPath(width, height, nodeRect, options.rectResolutionScale);

            var svg = new StringBuilder();
            svg.Append($@"<svg ");
            AppendSvgSizeAndViewBox(svg, viewBox);
            svg.Append($@"fill=""none"" ");
            svg.AppendLine($@"xmlns=""http://www.w3.org/2000/svg"">");
            
            AppendSvgFillPathElement(svg, node, path, 0, new Vector2(width, height), fills);

            if (strokeWeight > 0)
            {
                AppendSvgStrokeRectElement(svg, node, path, 0, new Vector2(width, height), options.rectResolutionScale);
            }

            svg.AppendLine("</svg>");
            
            return new SvgString(node, svg.ToString())
            {
                dontResize = dontResize
            };
        }

        private static void AppendSvgSizeAndViewBox(StringBuilder svg, Rect viewBox)
        {
            svg.Append($@"width=""{viewBox.width.ToString(CultureInfo.InvariantCulture)}"" ");
            svg.Append($@"height=""{viewBox.height.ToString(CultureInfo.InvariantCulture)}"" ");
            
            svg.Append($@"viewBox=""");
            svg.Append($@"{viewBox.x.ToString(CultureInfo.InvariantCulture)} ");
            svg.Append($@"{viewBox.y.ToString(CultureInfo.InvariantCulture)} ");
            svg.Append($@"{viewBox.width.ToString(CultureInfo.InvariantCulture)} ");
            svg.Append($@"{viewBox.height.ToString(CultureInfo.InvariantCulture)}"" ");
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

        private static Sprite GenerateRectSpriteFromSvg(SceneNode node, string svg, bool dontResize,
             SpriteGenerateOptions options)
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
                Mathf.Max(nodeRect.topLeftRadius, nodeRect.bottomLeftRadius) + strokePadding/2,
                Mathf.Max(nodeRect.bottomLeftRadius, nodeRect.bottomRightRadius) + strokePadding/2,
                Mathf.Max(nodeRect.topRightRadius, nodeRect.bottomRightRadius) + strokePadding/2,
                Mathf.Max(nodeRect.topLeftRadius, nodeRect.topRightRadius) + strokePadding/2
            );

            borders *= options.rectResolutionScale;
            
            // add 1pixel for each border to prevent bleeding
            borders += new Vector4(1, 1, 1, 1);

            var sceneInfo = ImportSvg(svg);
            return CreateSprite(node, options, sceneInfo, dontResize, borders);
        }

        private static void AppendSvgGradientStops(StringBuilder svg, GradientPaint gradient)
        {
            foreach (var gradientStop in gradient.gradientStops)
            {
                svg.Append($@"<stop ");
                svg.Append($@"offset=""{gradientStop.position.ToString(CultureInfo.InvariantCulture)}"" ");
                svg.Append($@"stop-color=""{gradientStop.color.ToString("rgb-hex")}"" ");
                svg.Append($@"stop-opacity=""{gradientStop.color.a.ToString(CultureInfo.InvariantCulture)}"" ");
                svg.AppendLine("/>");
            }
        }

        private static void AppendSvgGradient(StringBuilder svg, SceneNode node, GradientPaint gradient,
            int geometryIndex, int fillIndex, Vector2 viewSize, bool isStroke)
        {
            var type = isStroke ? "stroke" : "fill";
            var gradientId = $"{GetGradientId(node.id, type, geometryIndex, fillIndex)}" + 
                             $"{gradient.type.ToString()}";
            svg.AppendLine($@"{type}=""url(#{gradientId})"" />");

            if (gradient is LinearGradientPaint)
            {
                // Handles are normalized. So un-normalize them.
                var p1 = Vector2.Scale(gradient.gradientHandlePositions[0], viewSize);
                var p2 = Vector2.Scale(gradient.gradientHandlePositions[1], viewSize);

                svg.AppendLine(@"<defs>");
                svg.Append($@"<linearGradient ");
                svg.Append($@"id=""{gradientId}"" ");
                svg.Append($@"x1=""{p1.x.ToString(CultureInfo.InvariantCulture)}"" ");
                svg.Append($@"y1=""{p1.y.ToString(CultureInfo.InvariantCulture)}"" ");
                svg.Append($@"x2=""{p2.x.ToString(CultureInfo.InvariantCulture)}"" ");
                svg.Append($@"y2=""{p2.y.ToString(CultureInfo.InvariantCulture)}"" ");
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
                svg.Append($@"id=""{gradientId}"" ");
                svg.Append($@"cx=""0"" cy=""0"" r=""1"" ");
                svg.Append($@"gradientUnits=""userSpaceOnUse"" ");
                svg.Append($@"gradientTransform=""");
                svg.Append($@"translate({p1.x.ToString(CultureInfo.InvariantCulture)} {p1.y.ToString(CultureInfo.InvariantCulture)}) ");
                svg.Append($@"rotate({angle.ToString(CultureInfo.InvariantCulture)})");
                svg.Append($@"scale({sx.ToString(CultureInfo.InvariantCulture)} {sy.ToString(CultureInfo.InvariantCulture)})""");
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

        private static List<Paint> GetSvgFills(SceneNode node, SceneNode overrideNode, PaintOverride paintOverride)
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
            
            return fills ?? new List<Paint>();
        }

        private static void AppendSvgFillPathElement(StringBuilder svg, SceneNode node, string fillPath,
            int geometryIndex, Vector2 size, List<Paint> fills, WindingRule? windingRule = null)
        {
            for (var fillIndex = 0; fillIndex < fills.Count; fillIndex++)
            {
                var fill = fills[fillIndex];

                if (!fill.visible)
                    continue;

                svg.Append($@"<path d=""{fillPath}"" ");
                svg.Append($@"fill-opacity=""{fill.opacity.ToString(CultureInfo.InvariantCulture)}"" ");

                if (windingRule.HasValue && windingRule.Value != WindingRule.None)
                {
                    var windingRuleString = GetWindingRuleString(windingRule.Value);
                    svg.Append($@"fill-rule=""{windingRuleString}"" ");
                    svg.Append($@"clip-rule=""{windingRuleString}"" ");
                }

                if (fill is SolidPaint solid)
                {
                    AppendSvgSolid(svg, node, solid, size, geometryIndex, fillIndex, false, false, windingRule);
                }
                else if (fill is GradientPaint gradient)
                {
                    AppendSvgGradient(svg, node, gradient, geometryIndex, fillIndex, size, false);
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
            svg.Append($@"x1=""{p1.x.ToString(CultureInfo.InvariantCulture)}"" ");
            svg.Append($@"y1=""{p1.y.ToString(CultureInfo.InvariantCulture)}"" ");
            svg.Append($@"x2=""{p2.x.ToString(CultureInfo.InvariantCulture)}"" ");
            svg.Append($@"y2=""{p2.y.ToString(CultureInfo.InvariantCulture)}"" ");
            
            svg.Append($@"gradientUnits=""userSpaceOnUse"" ");
            svg.AppendLine($@">");

            svg.Append($@"<stop ");
            svg.Append($@"offset=""0"" ");
            svg.Append($@"stop-color=""{solid.color.ToString("rgb-hex")}"" ");
            svg.Append($@"stop-opacity=""{solid.opacity.ToString(CultureInfo.InvariantCulture)}"" ");
            svg.AppendLine("/>");

            svg.Append($@"<stop ");
            svg.Append($@"offset=""1"" ");
            svg.Append($@"stop-color=""{solid.color.ToString("rgb-hex")}"" ");
            svg.Append($@"stop-opacity=""{solid.opacity.ToString(CultureInfo.InvariantCulture)}"" ");
            svg.AppendLine("/>");

            svg.AppendLine(@"</linearGradient>");
            svg.AppendLine(@"</defs>");
        }

        private static void AppendSvgStrokePathElement(StringBuilder svg, SceneNode node, string strokePath,
            int geometryIndex, Vector2 size, SceneNode overrideNode = null)
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

            for (var strokeIndex = 0; strokeIndex < strokes.Count; strokeIndex++)
            {
                var stroke = strokes[strokeIndex];

                if (!stroke.visible)
                    continue;

                svg.Append($@"<path d=""{strokePath}"" ");

                if (stroke is SolidPaint solid)
                {
                    AppendSvgSolid(svg, node, solid, size, geometryIndex, strokeIndex, true, true);
                }
                else if (stroke is GradientPaint gradient)
                {
                    AppendSvgGradient(svg, node, gradient, geometryIndex, strokeIndex, size, true);
                }
                else
                {
                    svg.AppendLine("/>");
                }
            }
        }

        private static void AppendSvgSolid(StringBuilder svg, SceneNode node, SolidPaint solid, Vector2 size,
            int geometryIndex, int fillIndex, bool isStroke, bool isStrokeOutline, 
            WindingRule? windingRule = null)
        {
            if (isStroke && !isStrokeOutline)
            {
                svg.AppendLine($@"stroke=""{solid.color.ToString("rgb-hex")}"" />");
            }
            else
            {
                svg.AppendLine($@"fill=""{solid.color.ToString("rgb-hex")}"" />");
            }
        }

        private static string GetGradientId(string nodeId, string prefix, int geometryIndex, int fillIndex)
        {
            return $"{prefix}_{geometryIndex}_{fillIndex}_solid_{NodeUtils.HyphenateNodeID(nodeId)}";
        }

        private static void AppendSvgStrokeRectElement(StringBuilder svg, SceneNode node, string strokePath,
            int geometryIndex, Vector2 size, float strokeWidthMultiplier)
        {
            var nodeFill = (INodeFill)node;
            Debug.Assert(nodeFill != null);

            var strokeAlign = SvgHelpers.GetSvgValue(nodeFill.strokeAlign);
            var strokeWidth = (nodeFill.strokeWeight ?? 0) * strokeWidthMultiplier;
            for (var strokeIndex = 0; strokeIndex < nodeFill.strokes.Count; strokeIndex++)
            {
                var stroke = nodeFill.strokes[strokeIndex];

                if (!stroke.visible)
                    continue;

                svg.Append($@"<path d=""{strokePath}"" ");
                svg.Append($@"stroke-width=""{strokeWidth.ToString(CultureInfo.InvariantCulture)}"" ");
                svg.Append($@"stroke-opacity=""{stroke.opacity.ToString(CultureInfo.InvariantCulture)}"" ");
                svg.Append($@"stroke-alignment=""{strokeAlign}"" ");

                if (nodeFill.strokeDashes != null)
                {
                    var dashArrayStr = string.Join(' ',
                        nodeFill.strokeDashes.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                    svg.Append($@"stroke-dasharray=""{dashArrayStr}"" ");
                }

                if (stroke is SolidPaint solid)
                {
                    AppendSvgSolid(svg, node, solid, size, geometryIndex, strokeIndex, true, false);
                }
                else if (stroke is GradientPaint gradient)
                {
                    AppendSvgGradient(svg, node, gradient, geometryIndex, strokeIndex, size, true);
                }
                else
                {
                    svg.AppendLine("/>");
                }
            }
        }

        private static (Vector2, float) GetScaledSize(Sprite sprite, SpriteGenerateOptions options)
        {
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

            return (new Vector2(widthScaled, heightScaled), sizeRatio);
        }

        private static Sprite CreateSprite(SceneNode node, SpriteGenerateOptions options,
            SVGParser.SceneInfo sceneInfo, bool dontResize = false, Vector4? borders = null)
        {
            var geometries =
                VectorUtils.TessellateScene(sceneInfo.Scene, options.tessellationOptions, sceneInfo.NodeOpacity);

            var sprite = VectorUtils.BuildSprite(geometries, sceneInfo.SceneViewport,
                options.pixelsPerUnit, VectorUtils.Alignment.Center,
                Vector2.zero, options.gradientResolution, true);

            if (sprite == null)
                return null;

            var size = new Vector2(sprite.rect.width, sprite.rect.height);
            var sizeRatio = 1f;
            
            if (!dontResize)
            {
                (size, sizeRatio) = GetScaledSize(sprite, options);
            }
            
            var textureWidth = Mathf.RoundToInt(Mathf.Max(size.x, 1f));
            var textureHeight = Mathf.RoundToInt(Mathf.Max(size.y, 1f));

            var expandEdges = options.expandEdges && 
                              (options.filterMode != FilterMode.Point || options.sampleCount > 1);
            
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
            Texture2D texture, float scale = 1f,  Vector4? borders = null)
        {
            var spriteRect = new Rect(0, 0, texture.width, texture.height);
            var spritePivot = spriteRect.center;

            var pixelsPerUnit = options.pixelsPerUnit * scale * options.rectResolutionScale;

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

        private static string GetRectPath(float width, float height, INodeRect nodeRect, float rectCornerMultiplier)
        {
            var rect = new Rect(0, 0, width, height);
            var radiusTL = Vector2.one * nodeRect.topLeftRadius * rectCornerMultiplier;
            var radiusTR = Vector2.one * nodeRect.topRightRadius * rectCornerMultiplier;
            var radiusBR = Vector2.one * nodeRect.bottomRightRadius * rectCornerMultiplier;
            var radiusBL = Vector2.one * nodeRect.bottomLeftRadius * rectCornerMultiplier;
            var contour = VectorUtils.BuildRectangleContour(rect, radiusTL, radiusTR, radiusBR, radiusBL);
            var segments = contour.Segments;

            Debug.Assert(segments.Length > 1);

            var svg = new StringBuilder();
            svg.Append("M ");
            svg.Append($"{segments[0].P0.x.ToString(CultureInfo.InvariantCulture)} ");
            svg.Append($"{segments[0].P0.y.ToString(CultureInfo.InvariantCulture)} ");
            
            for (var i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                var segmentNext = i < segments.Length - 1 ? segments[i + 1] : segments[0];
                var control1 = segment.P1;
                var control2 = segment.P2;
                var end = segmentNext.P0;
                
                svg.Append("C ");
                svg.Append($"{control1.x.ToString(CultureInfo.InvariantCulture)} ");
                svg.Append($"{control1.y.ToString(CultureInfo.InvariantCulture)} ");
                svg.Append($"{control2.x.ToString(CultureInfo.InvariantCulture)} ");
                svg.Append($"{control2.y.ToString(CultureInfo.InvariantCulture)} ");
                svg.Append($"{end.x.ToString(CultureInfo.InvariantCulture)} ");
                svg.Append($"{end.y.ToString(CultureInfo.InvariantCulture)} ");
            }

            svg.Append("Z");
            return svg.ToString();
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
        
        private class SvgString
        {
            public SceneNode node { get; }
            public string content { get; }
            public bool dontResize { get; set; }

            public SvgString(SceneNode node, string content)
            {
                this.node = node;
                this.content = content;
            }

            public override string ToString()
            {
                return content;
            }
        }
    }
    
}