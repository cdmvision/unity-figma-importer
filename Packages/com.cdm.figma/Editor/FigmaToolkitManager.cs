// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
//using ButtonConfigHelper = Microsoft.MixedReality.Toolkit.UI.ButtonConfigHelper;

namespace Cdm.Figma
{
    /// <summary>
    /// Main class that interfaces with the Figma API and rebuilds the Figma pages
    /// from MRTK prefabs.
    /// </summary>
    public class FigmaToolkitManager
    {
        private HttpClient client = null;
        public FigmaSettings settings;
        public FigmaToolkitData data;
        public FigmaToolkitManager(FigmaSettings _settings, FigmaToolkitData _data)
        {
            settings = _settings;
            settings.DefaultCustomMap = Resources.Load<FigmaToolkitCustomMap>("Custom Maps/DefaultMap");
            data = _data;
        }

        public async void RefreshFile(string fileID)
        {
            await GetFile(fileID);
            Debug.Log($"File: {fileID} Refreshed");
        }

        public async Task<string> GetFile(string figmaFileKey)
        {
            if (string.IsNullOrEmpty(settings.FigmaToken))
            {
                Debug.LogError("Figma Token missing");
                return null;
            }

            Debug.Log($"Getting {figmaFileKey} from REST API");
            
            try
            {
                var responseBody = 
                    await FigmaApi.GetFileAsTextAsync(new FigmaFileRequest(settings.FigmaToken, figmaFileKey));

                responseBody = Uri.UnescapeDataString(responseBody);

                string directory = $"{FigmaSettings.FigmaBasePath}/FigmaFiles";
                Directory.CreateDirectory($"{FigmaSettings.FigmaBasePath}/FigmaFiles");
                System.IO.File.WriteAllText($"{directory}/{figmaFileKey}.json", responseBody);
                AssetDatabase.Refresh();
                Debug.Log($"File:{figmaFileKey} Retrieved");
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"\nException Caught!\nMessage :{e.Message}");
            }

            return null;
        }

        public FigmaFile BuildFigmaResponse(string jsonResponse)
        {
            try
            {
                FigmaFile response = FigmaFile.FromText(jsonResponse);
                return response;
            }
            catch (Exception e)
            {
                Debug.Log($"Error Building Document: {e.Message}");
                throw;
            }
        }

        public DocumentNode GetDocument(FigmaFileAsset figmaFile)
        {
            FigmaFile response = BuildFigmaResponse(figmaFile.file.text);
            figmaFile.name = response.name;

            return response.document;
        }

        public void GetLocalFiles()
        {
            if (data.files == null)
            {
                data.files = new List<FigmaFileAsset>();
            }

            TextAsset[] files = Resources.LoadAll<TextAsset>("FigmaFiles");
            bool filesChanged = false;
            foreach (TextAsset t in files)
            {
                if (data.files.Exists(x => x.fileId == t.name) == false)
                {
                    FigmaFileAsset file = new FigmaFileAsset { fileId = t.name, file = t };
                    data.files.Add(file);
                    var doc = GetDocument(file);
                    filesChanged = true;
                }
            }
            if (filesChanged)
            {
                EditorUtility.SetDirty(FigmaToolkitData.EditorGetOrCreateData());
            }

        }

        public void DeleteLocalFile(string fileID)
        {
            data.files.Remove(data.files.Find(x => x.fileId == fileID));
            FileUtil.DeleteFileOrDirectory($"{FigmaSettings.FigmaBasePath}/FigmaFiles/{fileID}.meta");
            FileUtil.DeleteFileOrDirectory($"{FigmaSettings.FigmaBasePath}/FigmaFiles/{fileID}.json");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void BuildDocument(string documentName, List<Node> nodes)
        {
            GameObject documentRoot = GameObject.Find(documentName);
            if (documentRoot == null)
            {
                documentRoot = new GameObject();
                documentRoot.name = documentName;
            }

            // make the game object hierarchy

            foreach (Node item in nodes)
            {
                Build(item, documentRoot.transform);
            }
        }

        private void Build(Node document, Transform parent)
        {
            GameObject go = null;

            switch (document.type)
            {
                case NodeType.Canvas:
                    go = BuildCanvas((CanvasNode) document);
                    break;
                case NodeType.Frame:
                    go = BuildFrame((FrameNode) document);
                    break;
                case NodeType.Group:
                    go = BuildGroup((GroupNode) document);
                    break;
                case NodeType.Instance:
                    go = BuildInstance((InstanceNode) document, parent);
                    break;
                case NodeType.Rectangle:
                    go = BuildRectange((RectangleNode) document);
                    break;
                case NodeType.Text:
                    go = BuildText((TextNode) document);
                    break;
                case NodeType.Boolean:
                case NodeType.Component:
                case NodeType.ComponentSet:
                case NodeType.Document:
                case NodeType.Ellipse:
                case NodeType.Line:
                case NodeType.RegularPolygon:
                case NodeType.Slice:
                case NodeType.Star:
                case NodeType.Vector:
                default:
                    //Debug.Log($"{document.type} named {document.name} was not built");
                    break;
            }

            if (!go)
            {
                go = new GameObject();
                go.name = document.name;
            }

            go.transform.SetParent(parent, true);

            var children = document.GetChildren();
            if (children != null && document.type != NodeType.Instance)
            {
                if (document.type != NodeType.ComponentSet)
                {
                    foreach (var item in children)
                    {
                        Build(item, go?.transform);
                    }
                }
            }

            // Prevent top-level "Page N" from being turned off
            /*if (document.absoluteBoundingBox == null)
            {
                go.SetActive(true);
            }*/

            go.SetActive(!document.visible);
            go.name += $" [{document.type}]";
        }

        private GameObject BuildBase(Node document)
        {
            GameObject go = new GameObject(document.name);
            return go;
        }

        private GameObject BuildCanvas(CanvasNode document)
        {
            GameObject go = BuildBase(document);
            return go;
        }

        private GameObject BuildFrame(FrameNode document)
        {
            GameObject go = BuildBase(document);
            SetPosition(document, go);
            return go;
        }

        private GameObject BuildGroup(GroupNode document)
        {
            GameObject go = BuildBase(document);
            return go;
        }

        private GameObject BuildRectange(Node document)
        {
            GameObject go = BuildBase(document);
            return go;
        }
        private GameObject BuildText(TextNode document, FigmaToolkitCustomMap customMap = null)
        {
            if (customMap == null)
            {
                customMap = settings.DefaultCustomMap;
            }
            // Create TextMeshPro object
            // Assign text from TextNode
            // Assign fontsize from TextNode
            // Assign font from TextNode

            GameObject go = new GameObject(document.name);
            TextMeshPro tmp = go.AddComponent<TextMeshPro>();
            tmp.text = document.characters;
            tmp.fontSize = document.style.fontSize;
            tmp.font = customMap.defaultFont;

            // TextAlignHorizontal { Center, Justified, Left, Right };
            // Default == Left
            if (document.style.textAlignHorizontal == TextAlignHorizontal.Center)
            {
                tmp.alignment = TMPro.TextAlignmentOptions.Center;
            }
            else if (document.style.textAlignHorizontal == TextAlignHorizontal.Justified)
            {
                tmp.alignment = TMPro.TextAlignmentOptions.Justified;
            }
            else if (document.style.textAlignHorizontal == TextAlignHorizontal.Right)
            {
                tmp.alignment = TMPro.TextAlignmentOptions.Right;
            }



            // Applying scale
            go.transform.localScale = new Vector3(FigmaSettings.PositionScale * 10.0f, FigmaSettings.PositionScale * 10.0f, FigmaSettings.PositionScale * 10.0f);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new UnityEngine.Vector2(document.absoluteBoundingBox.width, document.absoluteBoundingBox.height) * 0.10f;

            // Positioning
            rect.position = document.absoluteBoundingBox.position * FigmaSettings.PositionScale;

            Vector3[] v = new Vector3[4];
            rect.GetWorldCorners(v);
            rect.Translate(rect.position - v[1]);

            return go;
        }

        private GameObject BuildInstance(InstanceNode node, Transform parent, FigmaToolkitCustomMap customMap = null)
        {
            if (customMap == null)
            {
                customMap = settings.DefaultCustomMap;
            }

            CustomMapItem mapItem;
            GameObject go;

            if (customMap.componentMap.TryGetValue(node.name, out mapItem))
            {
                if (mapItem.Prefab != null)
                {
                    go = UnityEngine.Object.Instantiate(mapItem.Prefab, node.absoluteBoundingBox.position * FigmaSettings.PositionScale, mapItem.Prefab.transform.rotation, parent);

                    // Post-Process.
                    PostProcess(node, mapItem, go);

                    go.transform.Translate(go.transform.position - GetTopLeft(go));

                    // Applying offset.
                    go.transform.Translate(mapItem.offset);
                    return go;
                }
                else
                {
                    Debug.Log($"{node.name} not found in prefab Library");
                    go = BuildBase(node);
                    SetPosition(node, go);
                    return go;
                }
            }
            else
            {
                Debug.Log($"{node.name} not found in prefab Library");
                go = BuildBase(node);
                SetPosition(node, go);
                return go;
            }
        }

        private void PostProcess(Node node, CustomMapItem mapItem, GameObject go)
        {
            /*switch (mapItem.ProcessType)
            {
                case PostProcessType.Default:
                    break;
                case PostProcessType.Button:
                    /*ButtonConfigHelper buttonConfig = go.GetComponent<Microsoft.MixedReality.Toolkit.UI.ButtonConfigHelper>();
                    if (buttonConfig != null)
                    {
                        buttonConfig.MainLabelText = GetText(node);
                    }
                    break;
                case PostProcessType.ButtonCollection:
                    // Get buttons in prefab
                    /*ButtonConfigHelper[] buttonConfigHelpers = go.transform.Find("ButtonCollection").GetComponentsInChildren<ButtonConfigHelper>();

                    //Get buttons in node children
                    Node[] buttonNodes = Array.FindAll(node.children, x => x.name == "Buttons");

                    //Get text in each button
                    List<string> nodeTexts = new List<string>();
                    if (buttonNodes != null)
                    {
                        foreach (Node item in buttonNodes)
                        {
                            foreach (Node child in item.children)
                            {
                                nodeTexts.Add(GetText(child));
                            }
                        }
                    }
                    //Assign appropriate text to button
                    for (int i = 0; i < buttonConfigHelpers.Length; i++)
                    {
                        buttonConfigHelpers[i].MainLabelText = nodeTexts[i];

                    }
                    break;
                case PostProcessType.Backplate:
                    go.transform.localScale = new Vector3(node.absoluteBoundingBox.width * FigmaSettings.PositionScale, node.absoluteBoundingBox.height * FigmaSettings.PositionScale, go.transform.localScale.z);
                    break;
                case PostProcessType.Slider:
                    // Get default size. Hardcoding for now.
                    float defaultWidth = 764f;
                    // Get current size.
                    float currentWidth = node.absoluteBoundingBox.width;
                    // Getting scaling factor.
                    // Using x for uniform scaling.
                    float scaleFactor = currentWidth / defaultWidth;
                    // Applying scale.
                    go.transform.localScale *= scaleFactor;
                    break;
                default:
                    break;
            }*/
        }

        private void SetPosition(Node document, GameObject go)
        {
            /*if (document.absoluteBoundingBox != null)
            {
                go.transform.localPosition = document.absoluteBoundingBox.position * FigmaSettings.PositionScale;
            }*/
        }

        private Vector3 GetTopLeft(GameObject go)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            Bounds bounds = renderers[0].bounds;
            // Add all the renderer bounds in the hierarchy 
            foreach (Renderer r in renderers) { bounds.Encapsulate(r.bounds); }
            return new Vector3(bounds.min.x, bounds.max.y);
        }

        private string GetText(Node node)
        {
            if (node == null)
            {
                return null;
            }
            
            if (node.type == NodeType.Text)
            {
                return ((TextNode) node).characters;
            }

            var children = node.GetChildren();
            if (children != null)
            {
                foreach (var child in children)
                {
                    string found = GetText(child);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }
    }
}