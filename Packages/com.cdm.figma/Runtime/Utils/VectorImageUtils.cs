using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VectorGraphics;
using UnityEngine;

namespace Cdm.Figma.Utils
{
    public class VectorImageUtils
    {
        public static Sprite CreateSprite(SceneNode sceneNode,
            VectorUtils.TessellationOptions? tessellationOptions = null)
        {
            tessellationOptions ??= new VectorUtils.TessellationOptions()
            {
                StepDistance = 2.0f,
                MaxCordDeviation = 0.5f,
                MaxTanAngleDeviation = 0.1f,
                SamplingStepSize = 0.01f
            };

            if (sceneNode is VectorNode vectorNode)
            {
                if (vectorNode is INodeRect)
                {
                    return CreateSpriteByRect(sceneNode, tessellationOptions.Value);
                }

                return CreateSpriteByVectorPath(vectorNode, tessellationOptions.Value);
            }

            return CreateSpriteByRect(sceneNode, tessellationOptions.Value);
        }

        private static Sprite CreateSpriteByVectorPath(VectorNode vectorNode,
            VectorUtils.TessellationOptions tessellationOptions)
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

            var sceneInfo = SVGParser.ImportSVG(new StringReader(svgString.ToString()));
            return CreateSpriteWithTexture(vectorNode, tessellationOptions, sceneInfo.Scene);
        }

        private static Sprite CreateSpriteByRect(SceneNode node, VectorUtils.TessellationOptions tessellationOptions)
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

            return CreateSpriteWithTexture(node, tessellationOptions, scene, borders);
        }

        private static Sprite CreateSpriteWithTexture(SceneNode node,
            VectorUtils.TessellationOptions tessellationOptions, Scene svg, Vector4? borders = null)
        {
            var geoms = VectorUtils.TessellateScene(svg, tessellationOptions);
            var sprite = VectorUtils.BuildSprite(geoms, 100f, VectorUtils.Alignment.TopLeft, Vector2.zero, 128, true);
            if (sprite == null)
                return null;
            
            var texture = VectorUtils.RenderSpriteToTexture2D(sprite, (int)sprite.rect.width, (int)sprite.rect.height,
                new Material(Shader.Find("Unlit/Vector")), 1);
            if (texture == null)
                return null;

            var spriteRect = new Rect(0, 0, texture.width, texture.height);
            var spritePivot = spriteRect.center;

            Sprite spriteWithTexture = null;
            if (borders.HasValue)
            {
                spriteWithTexture =
                    Sprite.Create(texture, spriteRect, spritePivot, 100, 0, SpriteMeshType.FullRect, borders.Value);
            }
            else
            {
                spriteWithTexture = Sprite.Create(texture, spriteRect, spritePivot, 100, 0);
            }

            spriteWithTexture.name = node.id;

            Object.DestroyImmediate(sprite);

            return spriteWithTexture;
        }
    }
}