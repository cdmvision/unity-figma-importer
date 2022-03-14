using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class VectorDrawer
    {
        private VectorUtils.TessellationOptions _tessellationOptions;

        public VectorDrawer()
        {
            _tessellationOptions = new VectorUtils.TessellationOptions()
            {
                StepDistance = 2.0f,
                MaxCordDeviation = 0.5f,
                MaxTanAngleDeviation = 0.1f,
                SamplingStepSize = 0.01f
            };
        }

        public VectorDrawer(VectorUtils.TessellationOptions tessOptions)
        {
            _tessellationOptions = tessOptions;
        }

        public Sprite DrawVector(VectorNode vectorNode)
        {
            float width = vectorNode.size.x;
            float height = vectorNode.size.y;
            string path, fillCode, strokeCode;
            SolidPaint fillColor, strokeColor;
            float strokeWidth;
            
            if (vectorNode.fillGeometry.Length > 0)
            {
                path = vectorNode.fillGeometry[0].path;
                fillColor = (SolidPaint) vectorNode.fills[0];
                fillCode = fillColor.color.ToString("rgb-hex");
            }
            else
            {
                path = "";
                fillCode = "#0000ffff";
            }
            
            if (vectorNode.strokes.Length > 0)
            {
                strokeColor = (SolidPaint) vectorNode.strokes[0];
                strokeCode = strokeColor.color.ToString("rgb-hex");
                strokeWidth = (float) vectorNode.strokeWeight;
            }
            else
            {
                strokeCode = "#0000ffff";
                strokeWidth = 0f;
            }
            
            string svg =
                $@"<svg width=""{width}"" height=""{height}"" xmlns=""http://www.w3.org/2000/svg"">
              
                <path d=""{path}"" fill=""{fillCode}"" stroke=""{strokeCode}"" stroke-width=""{strokeWidth}""/>
               
                </svg>";
            
            var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
            var geoms = VectorUtils.TessellateScene(sceneInfo.Scene, _tessellationOptions);
            var sprite = VectorUtils.BuildSprite(geoms, 100f, VectorUtils.Alignment.TopLeft, Vector2.zero, 128, true);
            Texture2D spriteTex = VectorUtils.RenderSpriteToTexture2D(sprite, (int)width, (int)height, new Material(Shader.Find("Unlit/Vector")),1);

            Vector4 borders = new Vector4(1, 1, 1, 1);
            
            return SaveAndLoadSprite(sprite, spriteTex, vectorNode.id, borders);
        }

        public Sprite DrawPseudoVector(GroupNode groupNode)
        {
            bool hasFill = false;
            bool hasStroke = false;
            var width = groupNode.size.x;
            var height = groupNode.size.y;
            Vector2 radiusTL = new Vector2(0, 0);
            Vector2 radiusTR = new Vector2(0, 0);
            Vector2 radiusBR = new Vector2(0, 0);
            Vector2 radiusBL = new Vector2(0, 0);

            if (groupNode.rectangleCornerRadii != null)
            {
                radiusTL = new Vector2(groupNode.rectangleCornerRadii[0], groupNode.rectangleCornerRadii[0]);
                radiusTR = new Vector2(groupNode.rectangleCornerRadii[1], groupNode.rectangleCornerRadii[1]);
                radiusBR = new Vector2(groupNode.rectangleCornerRadii[2], groupNode.rectangleCornerRadii[2]);
                radiusBL = new Vector2(groupNode.rectangleCornerRadii[3], groupNode.rectangleCornerRadii[3]);
            }
            else
            {
                if (groupNode.cornerRadius.HasValue)
                {
                    radiusTL = new Vector2((float) groupNode.cornerRadius, (float) groupNode.cornerRadius);
                    radiusTR = new Vector2((float) groupNode.cornerRadius, (float) groupNode.cornerRadius);
                    radiusBR = new Vector2((float) groupNode.cornerRadius, (float) groupNode.cornerRadius);
                    radiusBL = new Vector2((float) groupNode.cornerRadius, (float) groupNode.cornerRadius);
                }
            }

            SolidPaint fillColor = new SolidPaint();
            if (groupNode.fills.Count > 0)
            {
                hasFill = true;
                fillColor = (SolidPaint) groupNode.fills[0];
            }

            SolidPaint strokeColor = new SolidPaint();
            if (groupNode.strokes.Count > 0)
            {
                hasStroke = true;
                strokeColor = (SolidPaint) groupNode.strokes[0];
            }

            var strokeWidth = groupNode.strokeWeight;

            var rect = VectorUtils.BuildRectangleContour(new Rect(0, 0, width, height), radiusTL, radiusTR, radiusBR,
                radiusBL);
            var scene = new Scene()
            {
                Root = new Unity.VectorGraphics.SceneNode()
                {
                    Shapes = new List<Shape>
                    {
                        new Shape()
                        {
                            Contours = new BezierContour[] {rect},
                            Fill = new SolidFill()
                            {
                                Color = hasFill
                                    ? new UnityEngine.Color(fillColor.color.r, fillColor.color.g, fillColor.color.b)
                                    : UnityEngine.Color.clear,
                                Opacity = groupNode.opacity,
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
                                    HalfThickness = groupNode.strokeWeight.HasValue ? (float) strokeWidth : 0f
                                }
                            }
                        }
                    }
                }
            };
            
            var geoms = VectorUtils.TessellateScene(scene, _tessellationOptions);
            var sprite = VectorUtils.BuildSprite(geoms, 100f, VectorUtils.Alignment.TopLeft, Vector2.zero, 128, true);
            Texture2D spriteTex = VectorUtils.RenderSpriteToTexture2D(sprite, (int)width, (int)height, new Material(Shader.Find("Unlit/Vector")),1);
            
            Vector4 borders = groupNode.cornerRadius.HasValue
                ? new Vector4(groupNode.rectangleCornerRadii[0]*2, groupNode.rectangleCornerRadii[1]*2,
                    groupNode.rectangleCornerRadii[2]*2, groupNode.rectangleCornerRadii[3]*2)
                : new Vector4(1,1,1,1);
            
            return SaveAndLoadSprite(sprite, spriteTex, groupNode.id, borders);
        }

        private Sprite SaveAndLoadSprite(Sprite sprite, Texture2D spriteTex, string id, Vector4 borders)
        {
            string _spritesPath = "Resources/Figma/Sprites";
            var directoryToSprites = Path.Combine("Assets", _spritesPath);
            Directory.CreateDirectory(directoryToSprites);
            
            var spriteName = $"{id.Replace(":", "-").Replace(";", "_")}.png";
            var pathToSprite = Path.Combine(directoryToSprites, spriteName);
            
            File.WriteAllBytes(pathToSprite, spriteTex.EncodeToPNG());
            AssetDatabase.ImportAsset(pathToSprite, ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.Refresh();

            TextureImporter ti = AssetImporter.GetAtPath (pathToSprite) as TextureImporter;
            ti.spritePixelsPerUnit = sprite.pixelsPerUnit;
            ti.mipmapEnabled = false;
            ti.textureType = TextureImporterType.Sprite;
            
            TextureImporterSettings textureSettings = new TextureImporterSettings();
            ti.ReadTextureSettings(textureSettings);
            textureSettings.spriteMeshType = SpriteMeshType.FullRect;
            textureSettings.spriteExtrude = 1;
            textureSettings.spriteAlignment = (int)SpriteAlignment.TopLeft;
            textureSettings.spriteBorder = borders;
            
            ti.SetTextureSettings(textureSettings);
            EditorUtility.SetDirty(ti);
            ti.SaveAndReimport();
            
            return AssetDatabase.LoadAssetAtPath(pathToSprite, typeof (Sprite)) as Sprite;
        }
    }
}