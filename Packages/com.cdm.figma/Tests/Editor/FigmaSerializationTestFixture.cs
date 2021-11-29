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
            switch (node.type)
            {
                case NodeType.Boolean:
                    Assert.True(node is BooleanOperationNode, "BooleanOperationNode");
                    break;
                case NodeType.Canvas:
                    Assert.True(node is CanvasNode, "CanvasNode");
                    break;
                case NodeType.Component:
                    Assert.True(node is ComponentNode, "ComponentNode");
                    break;
                case NodeType.ComponentSet:
                    Assert.True(node is ComponentSetNode, "ComponentSetNode");
                    break;
                case NodeType.Document:
                    Assert.True(node is DocumentNode, "DocumentNode");
                    break;
                case NodeType.Ellipse:
                    Assert.True(node is EllipseNode, "EllipseNode");
                    break;
                case NodeType.Frame:
                    Assert.True(node is FrameNode, "FrameNode");
                    break;
                case NodeType.Group:
                    Assert.True(node is GroupNode, "GroupNode");
                    break;
                case NodeType.Instance:
                    Assert.True(node is InstanceNode, "InstanceNode");
                    break;
                case NodeType.Line:
                    Assert.True(node is LineNode, "LineNode");
                    break;
                case NodeType.Rectangle:
                    Assert.True(node is RectangleNode, "RectangleNode");
                    break;
                case NodeType.RegularPolygon:
                    Assert.True(node is RegularPolygonNode, "RegularPolygonNode");
                    break;
                case NodeType.Slice:
                    Assert.True(node is SliceNode, "SliceNode");
                    break;
                case NodeType.Star:
                    Assert.True(node is StarNode, "StarNode");
                    break;
                case NodeType.Text:
                    Assert.True(node is TextNode, "TextNode");
                    break;
                case NodeType.Vector:
                    Assert.True(node is VectorNode, "VectorNode");
                    break;
                default:
                    Assert.True(false, "Unknown node");
                    break;
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

            var effects = JsonConvert.DeserializeObject<Effect[]>(json, JsonSerializerHelper.CreateSettings());
            
            Assert.NotNull(effects);
            
            foreach (var effect in effects)
            {
                switch (effect.type)
                {
                    case EffectType.BackgroundBlur:
                        Assert.True(effect is BackgroundBlurEffect, "BackgroundBlur");
                        break;
                    case EffectType.LayerBlur:
                        Assert.True(effect is LayerBlurEffect, "LayerBlur");
                        break;
                    case EffectType.DropShadow:
                        Assert.True(effect is DropShadowEffect, "DropShadow");
                        break;
                    case EffectType.InnerShadow:
                        Assert.True(effect is InnerShadowEffect, "InnerShadow");
                        break;
                    default:
                        Assert.True(false, "Unknown effect");
                        break;
                }
            }
        }
    }
}