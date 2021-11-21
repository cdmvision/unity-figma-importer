// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma
{
    public partial class FigmaToolkitWindow
    {
        private VisualElement documentPanel;
        private VisualElement BuildDocumentPanel(FigmaFileAsset file = null)
        {
            List<Node> nodesToBuild = new List<Node>();
            var panel = new VisualElement();
            panel.style.paddingLeft = 15;
            panel.style.paddingRight = 6;
            panel.style.paddingTop = 2;
            panel.style.flexGrow = 1;

            if (file == null)
            {
                file = toolkitManager.data.files.Find(x => x.fileId == toolkitManager.data.lastDocumentOpened);
            }
            else { toolkitManager.data.lastDocumentOpened = file.fileId; }

            panel.name = "Document";
            var header = new Label("Document");
            header.AddToClassList("heading");

            var subheader = new Label("No file opened yet");
            subheader.style.overflow = Overflow.Hidden;
            subheader.style.textOverflow = TextOverflow.Ellipsis;
            var refreshButton = new Button() { text = "Refresh", name = "refreshbtn" };
            refreshButton.clicked += async () => { await toolkitManager.GetFile(file.fileId); BreadcrumbPop(); ShowPanel(BuildDocumentPanel()); };
            var webButton = new Button() { text = "View on web", name = "webbtn" };
            webButton.clicked += () => { Application.OpenURL($"https://www.figma.com/file/{file.fileId}"); };
            var buttons = new VisualElement();
            buttons.style.flexDirection = FlexDirection.Row;

            panel.Add(header);
            panel.Add(subheader);
            buttons.Add(refreshButton);
            buttons.Add(webButton);
            panel.Add(buttons);


            if (file != null)
            {
                subheader.text = file.name;
                var doc = toolkitManager.GetDocument(file);
                var subHeader = new Label($"Pages");
                subHeader.style.fontSize = 15f;
                subHeader.style.borderTopWidth = 10f;
                subHeader.style.borderBottomWidth = 5f;
                int fileIndex = toolkitManager.data.files.IndexOf(file);
                panel.Add(subHeader);
                Func<VisualElement> makeItem = () =>
                {
                    var item = new VisualElement();
                    item.style.flexDirection = FlexDirection.Row;
                    var toggle = new Toggle();
                    toggle.name = "selectToggle";
                    var label = new Label();


                    item.Add(toggle);
                    item.Add(label);
                    return item;
                };
                Action<VisualElement, int> bindItem = (e, i) =>
                {
                    if (e != null)
                    {
                        var toggle = e.Q<Toggle>();
                        toggle.RegisterValueChangedCallback(x =>
                        {
                            if (nodesToBuild.Contains(doc.children[i]))
                            {
                                nodesToBuild.Remove(doc.children[i]);
                            }
                            else
                            {
                                nodesToBuild.Add(doc.children[i]);
                            }
                        });
                        var label = e.Q<Label>();
                        label.text = doc.children[i].name;
                    }
                };
                const int itemHeight = 20;
                var listView = new ListView();
                listView.makeItem = makeItem;
                listView.itemsSource = doc.children;
                listView.bindItem = bindItem;
                listView.itemHeight = itemHeight;

                listView.style.flexGrow = 1.0f;
                listView.style.minHeight = 260;

                var buildButton = new Button(() => { toolkitManager.BuildDocument(file.name, nodesToBuild); }) { text = "Build Pages" };
                //buildButton.AddToClassList("panel25");

                panel.Add(listView);
                panel.contentContainer.Add(buildButton);
            }
            return panel;
        }
    }
}