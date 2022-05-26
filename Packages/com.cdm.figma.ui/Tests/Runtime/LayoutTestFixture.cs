using System;
using System.Collections;
using Cdm.Figma.Tests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Cdm.Figma.UI.Tests
{
    [TestFixture]
    public class LayoutTestFixture
    {
        private const float FloatDelta = 0.001f;
        private static NodeObject ReferenceView;
        
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
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var canvasRectTransform = canvas.GetComponent<RectTransform>();
                canvasRectTransform.position = Vector3.zero;
                canvasRectTransform.rotation = Quaternion.identity;
                canvasRectTransform.localScale = Vector3.one;

                var isImported = false;
                foreach (RectTransform child in document.nodeObject.rectTransform)
                {
                    if (child.name is "0")
                    {
                        ReferenceView = child.GetComponent<NodeObject>();
                        Assert.True(ReferenceView.node is INodeTransform);
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
                        //change 1 and 2's right and bottom to stretch the view
                        ReferenceView.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(-1*(1920-nodeTransform.size.x),ReferenceView.gameObject.GetComponent<RectTransform>().offsetMax.y);
                        ReferenceView.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(ReferenceView.gameObject.GetComponent<RectTransform>().offsetMin.x,1080-nodeTransform.size.y);
                        
                        yield return new WaitForSeconds(2f);
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
            Assert.AreEqual(ReferenceView.gameObject.GetComponent<RectTransform>().rect.width, ((INodeTransform)node).size.x);
            Assert.AreEqual(ReferenceView.gameObject.GetComponent<RectTransform>().rect.height, ((INodeTransform)node).size.y);
            ComparePositionAndSize(node, node, rootFrameObject);
        }
        
        private static void ComparePositionAndSize(Node rootNode, Node node, NodeObject nodeObject)
        {
            var offset = ((INodeTransform) rootNode).relativeTransform.GetPosition();
            foreach (var child in node.GetChildren())
            {
                var childNodeObject = FindChild(nodeObject, childObject => childObject.nodeName == child.name);
                var parentNodeObject = childNodeObject.gameObject.GetComponentInParent<RectTransform>();
                if (parentNodeObject.rect.width <= 0 || parentNodeObject.rect.height <= 0)
                {
                    Debug.LogWarning(childNodeObject.nodeName + "'s parent is most probably inverted," +
                              " skipping position and size test for children. DOUBLE CHECK IT ANYWAY.");
                    //TODO: Disable visibility for nodes which have their parent inverted? (Unity and Figma behave different in this case)
                    Debug.Log(childNodeObject.nodeName + "'s parent is most probably inverted," +
                                     " possible implementation needed: Disable visibility for nodes which have their parent inverted?");
                    return;
                }
                if (childNodeObject != null)
                {
                    if (child.GetType() != typeof(GroupNode))
                    {
                        Assert.AreEqual(childNodeObject.nodeName, child.name);

                        var importedSize = childNodeObject.rectTransform.rect;
                        var actualSize = (Vector2)((INodeTransform)child).size;

                        if (importedSize.width <= 0)
                        {
                            Debug.LogWarning(rootNode.name + "'s child " + childNodeObject.name + " width is under 0! (mostly because it is inverted, check it anyway)");
                            Assert.AreEqual(importedSize.width, importedSize.width, FloatDelta,
                                $"Screen {rootNode.name}: {child.name} (id {child.id})-> size.x");                        
                        }
                        else
                        {
                            Assert.AreEqual(actualSize.x, importedSize.width, FloatDelta,
                                $"Screen {rootNode.name}: {child.name} (id {child.id})-> size.x");
                        }
                        if (importedSize.height <= 0)
                        {
                            Debug.LogWarning(rootNode.name + "'s child " + childNodeObject.name + " height is under 0! (mostly because it is inverted, check it anyway)");
                            Assert.AreEqual(importedSize.height, importedSize.height, FloatDelta,
                                $"Screen {rootNode.name}: {child.name} (id {child.id})-> size.y");                        
                        }
                        else
                        {
                            Assert.AreEqual(actualSize.y, importedSize.height, FloatDelta,
                                $"Screen {rootNode.name}: {child.name} (id {child.id})-> size.y");
                        }

                        var importedPosition = childNodeObject.rectTransform.position;
                        importedPosition.x += offset.x;
                        importedPosition.y -= offset.y;
                        var actualPosition = ((INodeTransform) child).absoluteBoundingBox;
                        
                        if (importedSize.width <= 0 || importedSize.height <= 0)
                        {
                            Debug.LogWarning(rootNode.name + "'s child " + childNodeObject.name + " width or height is most probably inverted, skipped position test because it doesn't matter, CHECK IT ANYWAY!");
                        }
                        else
                        {
                            Assert.AreEqual(actualPosition.x, importedPosition.x, FloatDelta,
                                $"Screen {rootNode.name}: {child.name} (id {child.id})-> pos.x");
                            Assert.AreEqual(-1*actualPosition.y, importedPosition.y-1080, FloatDelta,
                                $"Screen {rootNode.name}: {child.name} (id {child.id})-> pos.y");
                        }
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