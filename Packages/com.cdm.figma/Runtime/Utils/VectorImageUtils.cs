using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VectorGraphics;
using UnityEngine;

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

        public static Sprite CreateSprite(SceneNode sceneNode, SpriteOptions options = null)
        {
            options ??= new SpriteOptions();
            
            if (sceneNode is VectorNode vectorNode)
            {
                if (vectorNode is INodeRect)
                {
                    return CreateSpriteByRect(sceneNode, options);
                }

                return CreateSpriteByVectorPath(vectorNode, options);
            }

            return CreateSpriteByRect(sceneNode, options);
        }

        private static Sprite CreateSpriteByVectorPath(VectorNode vectorNode, SpriteOptions options)
        {
            var width = vectorNode.size.x;
            var height = vectorNode.size.y;
            var strokeWidth = vectorNode.strokeWeight ?? 0;

            var svgString = new StringBuilder();
            svgString.AppendLine($@"<svg width=""{width}"" height=""{height}"" xmlns=""http://www.w3.org/2000/svg"">");

            for (var i = 0; i < vectorNode.fillGeometry.Length; i++)
            {
                var svgPath = vectorNode.fillGeometry[i].path;
                svgString.Append($@"<path d=""{svgPath}"" ");

                if (i < vectorNode.fills.Count && vectorNode.fills[i] is SolidPaint fill)
                {
                    svgString.Append($@"fill=""{fill.color.ToString("rgb-hex")}"" ");
                }

                if (i < vectorNode.strokes.Count && vectorNode.strokes[i] is SolidPaint stroke)
                {
                    svgString.Append($@"stroke=""{stroke.color.ToString("rgb-hex")}"" ");
                    svgString.Append($@"stroke-width=""{strokeWidth}"" ");
                }

                svgString.AppendLine("/>");
            }

            svgString.AppendLine("</svg>");

            var sceneInfo = SVGParser.ImportSVG(
                new StringReader(svgString.ToString()), ViewportOptions.PreserveViewport);
            return CreateSpriteWithTexture(vectorNode, options, sceneInfo.Scene);
        }

        private static Sprite CreateSpriteByRect(SceneNode node, SpriteOptions options)
        {
            var nodeTransform = (INodeTransform)node;
            var nodeBlend = (INodeBlend)node;
            var nodeRect = (INodeRect)node;

            if (nodeTransform == null || nodeBlend == null || nodeRect == null)
            {
                return null;
            }

            var hasFill = false;
            var hasStroke = false;
            var width = nodeTransform.size.x;
            var height = nodeTransform.size.y;

            var radiusTL = Vector3.one * nodeRect.topLeftRadius;
            var radiusTR = Vector3.one * nodeRect.topRightRadius;
            var radiusBR = Vector3.one * nodeRect.bottomRightRadius;
            var radiusBL = Vector3.one * nodeRect.bottomLeftRadius;


            var fillColor = new SolidPaint();
            if (nodeBlend.fills.Count > 0)
            {
                hasFill = true;
                fillColor = (SolidPaint)nodeBlend.fills[0];
            }

            var strokeColor = new SolidPaint();
            if (nodeBlend.strokes.Count > 0)
            {
                hasStroke = true;
                strokeColor = (SolidPaint)nodeBlend.strokes[0];
            }

            var strokeWidth = nodeBlend.strokeWeight ?? 0f;

            var rect = VectorUtils.BuildRectangleContour(
                new Rect(0, 0, width, height), radiusTL, radiusTR, radiusBR, radiusBL);
            var scene = new Scene()
            {
                Root = new Unity.VectorGraphics.SceneNode()
                {
                    Shapes = new List<Shape>
                    {
                        new Shape()
                        {
                            Contours = new BezierContour[] { rect },
                            Fill = new SolidFill()
                            {
                                Color = hasFill
                                    ? new UnityEngine.Color(fillColor.color.r, fillColor.color.g, fillColor.color.b)
                                    : UnityEngine.Color.clear,
                                Opacity = nodeBlend.opacity,
                                Mode = FillMode.NonZero
                            },

                            PathProps = new PathProperties()
                            {
                                Stroke = new Stroke()
                                {
                                    Color = hasStroke
                                        ? new UnityEngine.Color(strokeColor.color.r, strokeColor.color.g,
                                            strokeColor.color.b)
                                        : UnityEngine.Color.clear,
                                    HalfThickness = strokeWidth
                                }
                            }
                        }
                    }
                }
            };

            // Left, bottom, right and top.
            var borders = new Vector4(
                Mathf.Max(nodeRect.topLeftRadius, nodeRect.bottomLeftRadius, strokeWidth * 2),
                Mathf.Max(nodeRect.bottomLeftRadius, nodeRect.bottomRightRadius, strokeWidth * 2),
                Mathf.Max(nodeRect.topRightRadius, nodeRect.bottomRightRadius, strokeWidth * 2),
                Mathf.Max(nodeRect.topLeftRadius, nodeRect.topRightRadius, strokeWidth * 2)
            );

            return CreateSpriteWithTexture(node, options, scene, borders);
        }

        private static Sprite CreateSpriteWithTexture(
            SceneNode node, SpriteOptions options, Scene svg, Vector4? borders = null)
        {
            var geometries = VectorUtils.TessellateScene(svg, options.tessellationOptions);
            var sprite = VectorUtils.BuildSprite(geometries, options.svgPixelsPerUnit, VectorUtils.Alignment.TopLeft, 
                Vector2.zero, options.gradientResolution, true);
            if (sprite == null)
                return null;

            var widthRatio = options.textureSize / sprite.rect.width;
            var heightRatio = options.textureSize / sprite.rect.height;

            var ratio = Mathf.Min(widthRatio, heightRatio);
            
            var width = (int) (sprite.rect.width * ratio);
            var height = (int) (sprite.rect.height * ratio);

            var material = new Material(Shader.Find("Unlit/Vector"));
            var texture = VectorUtils.RenderSpriteToTexture2D(sprite, width, height, material, 1, true);

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

            Sprite spriteWithTexture = null;
            if (borders.HasValue)
            {
                borders *= ratio;
                spriteWithTexture = Sprite.Create(
                    texture, spriteRect, spritePivot, options.pixelsPerUnit, 0, SpriteMeshType.FullRect, borders.Value);
            }
            else
            {
                spriteWithTexture = Sprite.Create(texture, spriteRect, spritePivot, options.pixelsPerUnit, 0);
            }

            spriteWithTexture.name = node.id;

            return spriteWithTexture;
        }
    }
}