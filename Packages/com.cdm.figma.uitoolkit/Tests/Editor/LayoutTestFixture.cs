using System.IO;
using Cdm.Figma;
using Cdm.Figma.Tests;
using Cdm.Figma.UIToolkit;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests.Editor.UIToolkit
{
    [TestFixture]
    public class LayoutTestFixture
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var fileJson = AssetDatabase.LoadAssetAtPath<TextAsset>(GetFilePath("Layout.json"));

            var figmaImporter = new FigmaImporter();
            var figmaFile = figmaImporter.CreateFile("file-id", fileJson.text);
            figmaImporter.ImportFileAsync(figmaFile).WaitOrThrow();

            foreach (var document in figmaImporter.GetImportedDocuments())
            {
                Debug.Log(document.page.name);
            }
        }
        
        [OneTimeTearDown]
        public void Cleanup()
        {
        }
        
        [Test]
        public void AutoLayout_TopLeftConstraint()
        {
        }
        
        [Test]
        public void AutoLayout_CenterConstraint()
        {
        }
        
        private static string GetFilePath(string fileName)
        {
            return Path.Combine("Packages/com.cdm.figma/Tests/Editor/TestResources/", fileName);
        }
    }
}