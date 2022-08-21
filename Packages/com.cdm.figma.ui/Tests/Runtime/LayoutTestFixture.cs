using System;
using System.Collections;
using System.IO;
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
        private FigmaNode _referenceView;

        [UnityTest]
        public IEnumerator PlayModeSampleTestWithEnumeratorPasses()
        {
            var figmaFile = Resources.Load<TextAsset>("File");
            var file = FigmaFile.Parse(figmaFile.text);
            
            var figmaImporter = new FigmaImporter();
            var figmaDesign = (FigmaDesign) figmaImporter.ImportFile(file);

            //var documents = figmaImporter.GetImportedDocuments();

            foreach (var page in figmaDesign.document)
            {
                var canvasGo = new GameObject($"Canvas-{page.name}");
                var canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var canvasRectTransform = canvas.GetComponent<RectTransform>();
                canvasRectTransform.position = Vector3.zero;
                canvasRectTransform.rotation = Quaternion.identity;
                canvasRectTransform.localScale = Vector3.one;

                var isImported = false;
                foreach (RectTransform child in page.rectTransform)
                {
                    if (child.name is "0")
                    {
                        _referenceView = child.GetComponent<FigmaNode>();
                        Assert.True(_referenceView.node is INodeTransform);
                        child.SetParent(canvas.transform, false);
                        isImported = true;
                        break;
                    }
                }

                Assert.True(isImported);

                foreach (var child in page.node.GetChildren())
                {
                    if (child is INodeTransform nodeTransform)
                    {
                        //change 1 and 2's right and bottom to stretch the view
                        _referenceView.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(
                            -1 * (1920 - nodeTransform.size.x),
                            _referenceView.gameObject.GetComponent<RectTransform>().offsetMax.y);
                        _referenceView.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(
                            _referenceView.gameObject.GetComponent<RectTransform>().offsetMin.x,
                            1080 - nodeTransform.size.y);

                        yield return new WaitForSeconds(2f);
                        Foo(child, canvasRectTransform);
                    }
                }

                Object.DestroyImmediate(canvasGo);
            }
        }

        private void Foo(Node node, Transform canvas)
        {
            var rootFrameTransform = (INodeTransform) node;
            var rootFrameObject = canvas.GetChild(0).GetComponent<FigmaNode>();

            // Do not compare root frame node names.
            Assert.AreEqual(_referenceView.gameObject.GetComponent<RectTransform>().rect.width,
                ((INodeTransform) node).size.x);
            Assert.AreEqual(_referenceView.gameObject.GetComponent<RectTransform>().rect.height,
                ((INodeTransform) node).size.y);
            CompareSize(node, node, rootFrameObject);
        }

        private static void CompareSize(Node rootNode, Node node, FigmaNode nodeObject)
        {
            var offset = ((INodeTransform) rootNode).relativeTransform.GetPosition();
            foreach (var child in node.GetChildren())
            {
                var childNodeObject = FindChild(nodeObject, childObject => childObject.nodeName == child.name);
                var parentNodeObject = childNodeObject.gameObject.GetComponentInParent<RectTransform>();
                if ((parentNodeObject.rect.width <= 0 || parentNodeObject.rect.height <= 0) &&
                    parentNodeObject.name != childNodeObject.name)
                {
                    Debug.LogWarning(
                        $"{childNodeObject.nodeName}'s parent {parentNodeObject.name} is most probably inverted," +
                        " skipping position and size test for children. DOUBLE CHECK IT ANYWAY.");
                    //TODO: Disable visibility for nodes which have their parent inverted? (Unity and Figma behave different in this case)
                    Debug.Log($"{childNodeObject.nodeName}'s parent is most probably inverted," +
                              " possible implementation needed: Disable visibility for nodes which have their parent inverted?");
                    return;
                }

                if (childNodeObject != null)
                {
                    if (child.GetType() != typeof(GroupNode))
                    {
                        Assert.AreEqual(childNodeObject.nodeName, child.name);

                        var importedSize = childNodeObject.rectTransform.rect;
                        var actualSize = (Vector2) ((INodeTransform) child).size;

                        if (importedSize.width <= 0)
                        {
                            Debug.LogWarning(
                                $"{rootNode.name}'s child {childNodeObject.name}'s width is under 0! (mostly because it is inverted, check it anyway)");
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
                            Debug.LogWarning(
                                $"{rootNode.name}'s child {childNodeObject.name}'s height is under 0! (mostly because it is inverted, check it anyway)");
                            Assert.AreEqual(importedSize.height, importedSize.height, FloatDelta,
                                $"Screen {rootNode.name}: {child.name} (id {child.id})-> size.y");
                        }
                        else
                        {
                            Assert.AreEqual(actualSize.y, importedSize.height, FloatDelta,
                                $"Screen {rootNode.name}: {child.name} (id {child.id})-> size.y");
                        }

                        if (importedSize.width <= 0 || importedSize.height <= 0)
                        {
                            Debug.LogWarning(
                                $"{rootNode.name}'s child {childNodeObject.name}'s width or height is under 0! Skipping position testing... (mostly because it is inverted, check it anyway)");
                        }
                        else
                        {
                            ComparePosition(rootNode, child, childNodeObject, offset);
                        }
                        
                    }

                    CompareSize(rootNode, child, childNodeObject);
                }
            }
        }

        private static void ComparePosition(Node rootNode, Node child, FigmaNode childNodeObject, Vector2 offset)
        {
            var importedPosition = childNodeObject.rectTransform.position;
            importedPosition.x += offset.x;
            importedPosition.y -= offset.y;
            var actualPosition = ((INodeTransform) child).absoluteBoundingBox;

            Assert.AreEqual(actualPosition.x, importedPosition.x, FloatDelta,
                $"Screen {rootNode.name}: {child.name} (id {child.id})-> pos.x");
            Assert.AreEqual(-1 * actualPosition.y, importedPosition.y - 1080, FloatDelta,
                $"Screen {rootNode.name}: {child.name} (id {child.id})-> pos.y");
            
        }

        private static FigmaNode FindChild(FigmaNode nodeObject, Predicate<FigmaNode> predicate)
        {
            foreach (Transform child in nodeObject.transform)
            {
                var childNode = child.GetComponent<FigmaNode>();

                if (predicate(childNode))
                {
                    return childNode;
                }
            }

            return null;
        }
        
        private static string GetFilePath(string fileName)
        {
            return Path.Combine("Packages/com.cdm.figma/Tests/Editor/TestResources/", fileName);
        }
    }
}