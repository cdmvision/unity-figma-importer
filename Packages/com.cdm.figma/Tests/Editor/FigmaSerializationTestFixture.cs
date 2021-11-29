using System;
using System.Collections.Generic;
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
            var testFile =
                AssetDatabase.LoadAssetAtPath<TextAsset>(
                    "Packages/com.cdm.figma/Tests/Editor/TestResources/TestFile.json");

            var figmaFile = FigmaFile.FromString(testFile.text);

            CheckNodeTypes(figmaFile.document);
        }

        private static void CheckNodeTypes(Node node)
        {
            var nodes = new Dictionary<string, Type>()
            {
                {NodeType.Boolean, typeof(BooleanOperationNode)},
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
            const string json = @"[
                                {
                                    'type': 'BACKGROUND_BLUR',
                                    'visible': true,
                                },
                                {
                                    'type': 'LAYER_BLUR',
                                    'visible': true,
                                },
                                {
                                    'type': 'INNER_SHADOW',
                                    'visible': true,
                                },
                                {
                                    'type': 'DROP_SHADOW',
                                    'visible': true,
                                },
                            ]";

            var effectTypes = new Dictionary<string, Type>()
            {
                {EffectType.InnerShadow, typeof(InnerShadowEffect)},
                {EffectType.DropShadow, typeof(DropShadowEffect)},
                {EffectType.LayerBlur, typeof(LayerBlurEffect)},
                {EffectType.BackgroundBlur, typeof(BackgroundBlurEffect)}
            };
            
            var effects = JsonConvert.DeserializeObject<Effect[]>(json, JsonSerializerHelper.CreateSettings());
            
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
    }
}