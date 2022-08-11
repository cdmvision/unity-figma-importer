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

        private static void AppendSvgPathElement(StringBuilder svg, INodeFill nodeFill, string path, 
            string windingRule = null)
        {
            Debug.Assert(nodeFill != null);
            
            foreach (var paint in nodeFill.fills)
            {
                if (paint.visible && paint is SolidPaint fill)
                {
                    svg.Append($@"<path d=""{path}"" ");
                    
                    if (!string.IsNullOrEmpty(windingRule))
                    {
                        svg.Append($@"fill-rule=""{windingRule.ToLowerInvariant()}"" ");
                        svg.Append($@"clip-rule=""{windingRule.ToLowerInvariant()}"" ");
                    }
                    
                    svg.Append($@"fill=""{fill.color.ToString("rgb-hex")}"" ");
                    svg.Append($@"fill-opacity=""{fill.opacity}"" ");
                    svg.AppendLine("/>");
                }
            }

            var strokeAlign = SvgHelpers.GetSvgValue(nodeFill.strokeAlign);
            var strokeWidth = nodeFill.strokeWeight ?? 0;
            foreach (var stroke in nodeFill.strokes)
            {
                if (stroke.visible && stroke is SolidPaint paint)
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
            }
        }
        
        public static Sprite CreateSpriteFromPath(VectorNode vectorNode, SpriteOptions options = null)
        {
            options ??= new SpriteOptions();

            var width = vectorNode.size.x;
            var height = vectorNode.size.y;

            var svgString = new StringBuilder();
            svgString.AppendLine($@"<svg width=""{width}"" height=""{height}"" xmlns=""http://www.w3.org/2000/svg"">");

            foreach (var geometry in vectorNode.fillGeometry)
            {
                var path = geometry.path;
                var windingRule = geometry.windingRule;
                AppendSvgPathElement(svgString, vectorNode, path, windingRule);
            }

            svgString.AppendLine("</svg>");

            //Debug.Log($"{vectorNode.id} ({vectorNode.name}): {svgString}");

            return CreateSpriteFromSvg(vectorNode, svgString.ToString(), options);
        }

        public static Sprite CreateSpriteFromRect(SceneNode node, SpriteOptions options = null)
        {
            return CreateSpriteSceneRect(node, options);
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

            AppendSvgPathElement(svgString, nodeFill, path);
            
            svgString.AppendLine("</svg>");

            //Debug.Log($"{node.id}: {svgString}");
            
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