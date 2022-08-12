using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VectorGraphics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cdm.Figma.Utils
{
    public class VectorImageUtils
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

        public static Sprite CreateSpriteFromSvg(VectorNode vectorNode, string svg, SpriteOptions options = null)
        {
            options ??= new SpriteOptions();
            var sceneInfo = SVGParser.ImportSVG(
                new StringReader(svg), ViewportOptions.PreserveViewport);
            return CreateSpriteWithTexture(vectorNode, options, sceneInfo.Scene, sceneInfo);
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
                svg.Append($@">");

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
                var paint = nodeFill.fills[i];
                
                if (!paint.visible)
                    continue;
                
                if (paint is SolidPaint solid)
                {
                    svg.Append($@"<path d=""{path}"" ");
                    if (!string.IsNullOrEmpty(windingRule))
                    {
                        svg.Append($@"fill-rule=""{windingRule.ToLowerInvariant()}"" ");
                        svg.Append($@"clip-rule=""{windingRule.ToLowerInvariant()}"" ");
                    }
                    
                    svg.Append($@"fill=""{solid.color.ToString("rgb-hex")}"" ");
                    svg.Append($@"fill-opacity=""{paint.opacity}"" ");
                    svg.AppendLine("/>");
                    
                }
                else if (paint is GradientPaint gradient)
                {
                    var gradientID = $"fill{i}_{gradient.type.ToLowerInvariant()}_{NodeUtils.HyphenateNodeID(node.id)}";
                    svg.Append($@"<path d=""{path}"" ");
                    svg.Append($@"fill=""url(#{gradientID})"" ");
                    svg.Append($@"fill-opacity=""{paint.opacity}"" ");
                    svg.AppendLine("/>");

                    AppendSvgGradient(svg, gradient, gradientID, size);
                }
            }

            var strokeAlign = SvgHelpers.GetSvgValue(nodeFill.strokeAlign);
            var strokeWidth = nodeFill.strokeWeight ?? 0;
            for (var i = 0; i < nodeFill.strokes.Count; i++)
            {
                var stroke = nodeFill.strokes[i];
                
                if (!stroke.visible)
                    continue;
                
                if (stroke is SolidPaint paint)
                {
                    svg.Append($@"<path d=""{path}"" ");
                    
                    if (!string.IsNullOrEmpty(windingRule))
                    {
                        svg.Append($@"fill-rule=""{windingRule.ToLowerInvariant()}"" ");
                        svg.Append($@"clip-rule=""{windingRule.ToLowerInvariant()}"" ");
                    }
                    
                    svg.Append($@"fill=""none"" ");
                    svg.Append($@"stroke=""{paint.color.ToString("rgb-hex")}"" ");
                    svg.Append($@"stroke-width=""{strokeWidth}"" ");
                    svg.Append($@"stroke-opacity=""{paint.opacity}"" ");
                    svg.Append($@"stroke-alignment=""{strokeAlign}"" ");
                    svg.AppendLine("/>");
                }
                else if (stroke is GradientPaint gradient)
                {
                    var gradientID = $"stroke{i}_{gradient.type.ToLowerInvariant()}_{NodeUtils.HyphenateNodeID(node.id)}";
                    svg.Append($@"<path d=""{path}"" ");
                    svg.Append($@"fill=""none"" ");
                    svg.Append($@"stroke=""url(#{gradientID})"" ");
                    svg.Append($@"stroke-width=""{strokeWidth}"" ");
                    svg.Append($@"stroke-opacity=""{stroke.opacity}"" ");
                    svg.AppendLine("/>");

                    AppendSvgGradient(svg, gradient, gradientID, size);
                }
            }
        }
        
        public static Sprite CreateSpriteFromPath(VectorNode node, SpriteOptions options = null)
        {
            options ??= new SpriteOptions();

            var width = node.size.x;
            var height = node.size.y;

            var svgString = new StringBuilder();
            svgString.AppendLine($@"<svg width=""{width}"" height=""{height}"" xmlns=""http://www.w3.org/2000/svg"">");

            foreach (var geometry in node.fillGeometry)
            {
                var path = geometry.path;
                var windingRule = geometry.windingRule;
                AppendSvgPathElement(svgString, node, path, new Vector2(width, height), windingRule);
            }

            svgString.AppendLine("</svg>");

            //Debug.Log($"{vectorNode.id} ({vectorNode.name}): {svgString}");

            return CreateSpriteFromSvg(node, svgString.ToString(), options);
        }

        public static Sprite CreateSpriteFromRect(SceneNode node, SpriteOptions options = null)
        {
            //return CreateSpriteSceneRect(node, options);
            return CreateSpriteSvgRect(node, options);
        }

        private static Sprite CreateSpriteSceneRect(SceneNode node, SpriteOptions options = null)
        {
            options ??= new SpriteOptions();

            var nodeTransform = (INodeTransform)node;
            var nodeFill = (INodeFill)node;
            var nodeRect = (INodeRect)node;

            if (nodeTransform == null || nodeFill == null || nodeRect == null)
            {
                return null;
            }

            var scene = new Scene()
            {
                Root = new Unity.VectorGraphics.SceneNode()
                {
                    Shapes = new List<Shape>(),
                }
            };

            var rect = new Rect(0, 0, nodeTransform.size.x, nodeTransform.size.y);
            var radiusTL = Vector2.one * nodeRect.topLeftRadius;
            var radiusTR = Vector2.one * nodeRect.topRightRadius;
            var radiusBR = Vector2.one * nodeRect.bottomRightRadius;
            var radiusBL = Vector2.one * nodeRect.bottomLeftRadius;
            var rectContour = VectorUtils.BuildRectangleContour(rect, radiusTL, radiusTR, radiusBR, radiusBL);

            foreach (var paint in nodeFill.fills)
            {
                if (paint.visible)
                {
                    var fill = CreateShapeFill(paint, out var angle);

                    var shape = new Shape()
                    {
                        Contours = new BezierContour[] { rectContour },
                        Fill = fill,
                        FillTransform = Matrix2D.RotateLH(angle * Mathf.Deg2Rad),
                        IsConvex = true
                    };

                    scene.Root.Shapes.Add(shape);
                }
            }

            // Add strokes at top of fills.
            var strokeWidth = nodeFill.strokeWeight ?? 0f;
            foreach (var stroke in nodeFill.strokes)
            {
                var fill = CreateShapeFill(stroke, out var angle);
                var shape = new Shape()
                {
                    Contours = new BezierContour[] { rectContour },
                    IsConvex = true,
                    PathProps = new PathProperties()
                    {
                        Stroke = new Stroke()
                        {
                            Fill = fill,
                            FillTransform = Matrix2D.RotateLH(angle * Mathf.Deg2Rad),
                            HalfThickness = strokeWidth * 0.5f,
                            Pattern = nodeFill.strokeDashes
                        }
                    }
                };

                scene.Root.Shapes.Add(shape);
            }

            // Left, bottom, right and top.
            var strokePadding = strokeWidth * 2 + 4;
            var borders = new Vector4(
                Mathf.Max(nodeRect.topLeftRadius, nodeRect.bottomLeftRadius, strokePadding),
                Mathf.Max(nodeRect.bottomLeftRadius, nodeRect.bottomRightRadius, strokePadding),
                Mathf.Max(nodeRect.topRightRadius, nodeRect.bottomRightRadius, strokePadding),
                Mathf.Max(nodeRect.topLeftRadius, nodeRect.topRightRadius, strokePadding)
            );

            return CreateSpriteWithTexture(node, options, scene, null, borders);
        }
        
        private static Sprite CreateSpriteSvgRect(SceneNode node, SpriteOptions options = null)
        {
            options ??= new SpriteOptions();

            var nodeTransform = (INodeTransform)node;
            var nodeFill = (INodeFill)node;
            var nodeRect = (INodeRect)node;

            if (nodeTransform == null || nodeFill == null || nodeRect == null)
            {
                return null;
            }

            var width = nodeTransform.size.x;
            var height = nodeTransform.size.y;
            
            var path = GetRectPath(nodeTransform, nodeRect);
            var svgString = new StringBuilder();
            svgString.AppendLine(
                $@"<svg id=""{node.id}"" width=""{width}"" height=""{height}"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">");

            AppendSvgPathElement(svgString, node, path, new Vector2(width, height));
            
            svgString.AppendLine("</svg>");

            Debug.Log($"{node}: {svgString}");
            
            // Left, bottom, right and top.
            var strokeWidth = nodeFill.strokeWeight ?? 0;
            var strokePadding = strokeWidth * 2 + 4;
            var borders = new Vector4(
                Mathf.Max(nodeRect.topLeftRadius, nodeRect.bottomLeftRadius, strokePadding),
                Mathf.Max(nodeRect.bottomLeftRadius, nodeRect.bottomRightRadius, strokePadding),
                Mathf.Max(nodeRect.topRightRadius, nodeRect.bottomRightRadius, strokePadding),
                Mathf.Max(nodeRect.topLeftRadius, nodeRect.topRightRadius, strokePadding)
            );

            var sceneInfo =
                SVGParser.ImportSVG(new StringReader(svgString.ToString()), ViewportOptions.PreserveViewport);
            
            return CreateSpriteWithTexture(node, options, sceneInfo.Scene, sceneInfo, borders);
        }

        private static IFill CreateShapeFill(Paint paint, out float angle)
        {
            IFill fill = null;
            angle = 0f;

            if (paint is SolidPaint solidPaint)
            {
                var solidFill = new SolidFill();
                solidFill.Color = solidPaint.color;
                solidFill.Opacity = solidPaint.opacity;
                fill = solidFill;
            }

            if (paint is GradientPaint gradientPaint)
            {
                var gradientFill = new GradientFill();
                gradientFill.Addressing = AddressMode.Clamp;
                gradientFill.Opacity = gradientPaint.opacity;

                var handleStart = (Vector2)gradientPaint.gradientHandlePositions[0];
                var handleEnd = (Vector2)gradientPaint.gradientHandlePositions[1];

                var direction = (handleEnd - handleStart).normalized;
                angle = Vector2.Angle(Vector2.right, direction);

                // TODO: gradient handles

                // Add gradient stops.
                var gradientStops = new List<GradientStop>();
                foreach (var gs in gradientPaint.gradientStops)
                {
                    var gradientStop = new GradientStop();
                    gradientStop.Color = gs.color;
                    gradientStop.StopPercentage = gs.position;
                    gradientStops.Add(gradientStop);
                }

                gradientFill.Stops = gradientStops.ToArray();

                if (gradientPaint is LinearGradientPaint)
                {
                    gradientFill.Type = GradientFillType.Linear;
                }
                else if (gradientPaint is RadialGradientPaint)
                {
                    gradientFill.Type = GradientFillType.Radial;
                }

                fill = gradientFill;
            }


            return fill;
        }

        private static Sprite CreateSpriteWithTexture(SceneNode node, SpriteOptions options, Scene svg,
            SVGParser.SceneInfo? sceneInfo = null, Vector4? borders = null)
        {
            var geometries = VectorUtils.TessellateScene(svg, options.tessellationOptions, sceneInfo?.NodeOpacity);
            var sprite = VectorUtils.BuildSprite(geometries, options.svgPixelsPerUnit, VectorUtils.Alignment.TopLeft,
                Vector2.zero, options.gradientResolution, true);
            
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

        private static string GetRectPath(INodeTransform transform, INodeRect rect)
        {
            var w = transform.size.x;
            var h = transform.size.y;

            // Kappa @see https://nacho4d-nacho4d.blogspot.com/2011/05/bezier-paths-rounded-corners-rectangles.html
            const float k = 0.552228474f;

            var rtl = rect.topLeftRadius;
            var rtr = rect.topRightRadius;
            var rbr = rect.bottomRightRadius;
            var rbl = rect.bottomLeftRadius;

            var rtlk = k * rtl;
            var rtrk = k * rtr;
            var rbrk = k * rbr;
            var rblk = k * rbl;
            
            // TODO: Use code below to generate rectangle path.
            /*var rectf = new Rect(0, 0, transform.size.x, transform.size.y);
            var radiusTL = Vector2.one * rect.topLeftRadius;
            var radiusTR = Vector2.one * rect.topRightRadius;
            var radiusBR = Vector2.one * rect.bottomRightRadius;
            var radiusBL = Vector2.one * rect.bottomLeftRadius;
            var rectContour = VectorUtils.BuildRectangleContour(rectf, radiusTL, radiusTR, radiusBR, radiusBL);*/

            var svgString = new StringBuilder();
            svgString.Append($@"M 0 {rtl} ");

            if (rtl > 0)
            {
                svgString.Append($@"C 0 {rtl - rtlk} {rtl - rtlk} {0} {rtl} 0 ");
            }

            svgString.Append($@"H {w - rtr}");

            if (rtr > 0)
            {
                svgString.Append($@"C {w - rtr + rtrk} 0 {w} {rtr - rtrk} {w} {rtr} ");
            }

            svgString.Append($@"V {h - rbr}");

            if (rbr > 0)
            {
                svgString.Append($@"C {w} {h - (rbr - rbrk)} {w - (rbr - rbrk)} {h} {w - rbr} {h} ");
            }

            svgString.Append($@"H {rbl}");

            if (rbl > 0)
            {
                svgString.Append($@"C {rbl - rblk} {h} {0} {h - (rbl - rblk)} {0} {h - rbl} ");
            }

            svgString.Append($@"V {rtl}");
            svgString.Append($@"Z");
            return svgString.ToString();
        }
    }
}