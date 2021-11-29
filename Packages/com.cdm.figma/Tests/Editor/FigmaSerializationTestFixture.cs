using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace Cdm.Figma.Tests
{
    [TestFixture]
    public class FigmaSerializationTestFixture
    {
        [Test]
        public void NodeDeserializationByType()
        {
            var json = AssetDatabase.LoadAssetAtPath<TextAsset>(GetFilePath("File.json"));
            var file = FigmaFile.FromString(json.text);

            Assert.NotNull(file);
            Assert.NotNull(file.document);
            
            CheckNodeTypes(file.document);
        }

        private static void CheckNodeTypes(Node node)
        {
            var nodes = new Dictionary<string, Type>()
            {
                {NodeType.Boolean, typeof(BooleanNode)},
                {NodeType.Canvas, typeof(CanvasNode)},
                {NodeType.Component, typeof(ComponentNode)},
                {NodeType.ComponentSet, typeof(ComponentSetNode)},
                {NodeType.Document, typeof(DocumentNode)},
                {NodeType.Ellipse, typeof(EllipseNode)},
                {NodeType.Frame, typeof(FrameNode)},
                {NodeType.Group, typeof(GroupNode)},
                {NodeType.Instance, typeof(InstanceNode)},
                {NodeType.Line, typeof(LineNode)},
                {NodeType.Rectangle, typeof(RectangleNode)},
                {NodeType.RegularPolygon, typeof(RegularPolygonNode)},
                {NodeType.Slice, typeof(SliceNode)},
                {NodeType.Star, typeof(StarNode)},
                {NodeType.Text, typeof(TextNode)},
                {NodeType.Vector, typeof(VectorNode)},
            };

            if (nodes.TryGetValue(node.type, out var type))
            {
                Assert.True(node.GetType() == type, node.type);
            }
            else
            {
                Assert.Fail($"Unknown node type: {node.type}");
            }
            
            var children = node.GetChildren();
            if (children != null)
            {
                foreach (var child in children)
                {
                    CheckNodeTypes(child);
                }
            }
        }

        [Test]
        public void EffectDeserializationByType()
        {
            var effectTypes = new Dictionary<string, Type>()
            {
                {EffectType.InnerShadow, typeof(InnerShadowEffect)},
                {EffectType.DropShadow, typeof(DropShadowEffect)},
                {EffectType.LayerBlur, typeof(LayerBlurEffect)},
                {EffectType.BackgroundBlur, typeof(BackgroundBlurEffect)}
            };
            
            var json = AssetDatabase.LoadAssetAtPath<TextAsset>(GetFilePath("Effects.json"));
            var effects = JsonConvert.DeserializeObject<Effect[]>(json.text, JsonSerializerHelper.CreateSettings());
            
            Assert.NotNull(effects);
            
            foreach (var effect in effects)
            {
                if (effectTypes.TryGetValue(effect.type, out var type))
                {
                    Assert.True(effect.GetType() == type, effect.type);
                }
                else
                {
                    Assert.Fail($"Unknown effect type: {effect.type}");
                }
            }
        }
        
        [Test]
        public void PaintDeserializationByType()
        {
            var paintTypes = new Dictionary<string, Type>()
            {
                {PaintType.Solid, typeof(SolidPaint)},
                {PaintType.GradientLinear, typeof(LinearGradientPaint)},
                {PaintType.GradientRadial, typeof(RadialGradientPaint)},
                {PaintType.GradientAngular, typeof(AngularGradientPaint)},
                {PaintType.GradientDiamond, typeof(DiamondGradientPaint)},
                {PaintType.Image, typeof(ImagePaint)}
            };
            
            var json = AssetDatabase.LoadAssetAtPath<TextAsset>(GetFilePath("Paints.json"));
            var paints = JsonConvert.DeserializeObject<Paint[]>(json.text, JsonSerializerHelper.CreateSettings());
            
            Assert.NotNull(paints);
            
            foreach (var paint in paints)
            {
                if (paintTypes.TryGetValue(paint.type, out var type))
                {
                    Assert.True(paint.GetType() == type, paint.type);
                }
                else
                {
                    Assert.Fail($"Unknown paint type: {paint.type}");
                }
            }
        }

        private static string GetFilePath(string fileName)
        {
            return Path.Combine("Packages/com.cdm.figma/Tests/Editor/TestResources/", fileName);
        }
    }
}