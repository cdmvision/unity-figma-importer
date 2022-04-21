using System;
using System.Collections;
using Cdm.Figma.Tests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Cdm.Figma.UI.Tests
{
    [TestFixture]
    public class LayoutTestFixture
    {
        private const float FloatDelta = 0.001f;
        
        [UnityTest]
        public IEnumerator PlayModeSampleTestWithEnumeratorPasses()
        {
            var figmaFile = Resources.Load<FigmaFile>("2282TYGl73YRNXVIB0kYYD");

            var figmaImporter = new FigmaImporter();
            yield return figmaImporter.ImportFileAsync(figmaFile).AsEnumerator();

            var documents = figmaImporter.GetImportedDocuments();

            foreach (var document in documents)
            {
                var canvasGo = new GameObject($"Canvas-{document.node.name}");
                var canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                var canvasRectTransform = canvas.GetComponent<RectTransform>();
                canvasRectTransform.position = Vector3.zero;
                canvasRectTransform.rotation = Quaternion.identity;
                canvasRectTransform.localScale = Vector3.one;

                var isImported = false;
                foreach (RectTransform child in document.nodeObject.rectTransform)
                {
                    if (child.name is "0")
                    {
                        var childNode = child.GetComponent<NodeObject>();

                        Assert.True(childNode.node is INodeTransform);

                        var nodeTransform = (INodeTransform)childNode.node;

                        canvasRectTransform.sizeDelta = nodeTransform.size;
                        child.SetParent(canvas.transform, false);
                        isImported = true;
                        break;
                    }
                }

                Assert.True(isImported);

                foreach (var child in document.node.GetChildren())
                {
                    if (child is INodeTransform nodeTransform)
                    {
                        canvasRectTransform.sizeDelta = nodeTransform.size;

                        Foo(child, canvasRectTransform);
                    }
                }

                Object.DestroyImmediate(canvasGo);
            }
        }

        private static void Foo(Node node, Transform canvas)
        {
            var rootFrameTransform = (INodeTransform)node;
            var rootFrameObject = canvas.GetChild(0).GetComponent<NodeObject>();

            // Do not compare root frame node names. 
            Assert.AreEqual(rootFrameObject.rectTransform.rect.size, (Vector2)((INodeTransform)node).size);

            ComparePositionAndSize(node, node, rootFrameObject);
        }
        
        private static void ComparePositionAndSize(Node rootNode, Node node, NodeObject nodeObject)
        {
            foreach (var child in node.GetChildren())
            {
                var childNodeObject = FindChild(nodeObject, childObject => childObject.nodeName == child.name);
                if (childNodeObject != null)
                {
                    if (child.GetType() != typeof(GroupNode))
                    {
                        Assert.AreEqual(childNodeObject.nodeName, child.name);

                        var importedSize = childNodeObject.rectTransform.rect;
                        var actualSize = (Vector2)((INodeTransform)child).size;

                        Assert.AreEqual(actualSize.x, Mathf.Abs(importedSize.x), FloatDelta,
                            $"{rootNode.name}:{child.name}:size.x");
                        Assert.AreEqual(actualSize.y, Mathf.Abs(importedSize.y), FloatDelta,
                            $"{rootNode.name}:{child.name}:size.y");
                    }
                    
                    ComparePositionAndSize(rootNode, child, childNodeObject);
                }
            }
        }

        private static NodeObject FindChild(NodeObject nodeObject, Predicate<NodeObject> predicate)
        {
            foreach (Transform child in nodeObject.transform)
            {
                var childNode = child.GetComponent<NodeObject>();

                if (predicate(childNode))
                {
                    return childNode;
                }
            }

            return null;
        }
    }
}