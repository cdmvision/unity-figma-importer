// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cdm.Figma
{
    public partial class FigmaToolkitWindow
    {
        private VisualElement filesPanel { get { return BuildFilesPanel(); } }

        private VisualElement BuildFilesPanel()
        {
            toolkitManager.GetLocalFiles();

            var panel = new VisualElement();
            panel.name = "Files";
            panel.style.paddingLeft = 15;
            panel.style.paddingRight = 6;
            panel.style.paddingTop = 2;
            panel.style.flexGrow = 1;

            var header = new Label("Files");
            header.AddToClassList("heading");
            header.AddToClassList("panel75");
            var subheader = new Label("Please enter file ID from the Figma URL");

            var getAllFilesButton = new Button(() =>
            {
                toolkitManager.GetLocalFiles();
                RefreshFilesPanel();
            })
            { text = "Refresh Files" };

            panel.Add(header);
            panel.Add(subheader);

            var fileRequestContainer = new VisualElement();
            fileRequestContainer.AddToClassList("horizontalcontainer");
            fileRequestContainer.style.borderBottomWidth = 10;
            var fileRequestField = new TextField() { tooltip = "Enter fileID" };
            fileRequestField.BindProperty(data.FindProperty("lastRequestedFile"));
            fileRequestField.AddToClassList("panel75");
            var fileRequestButton = new Button(async () => { await toolkitManager.GetFile(toolkitManager.data.lastRequestedFile); RefreshFilesPanel(); }) { text = "Get File" };
            fileRequestButton.AddToClassList("panel25");

            var scrollView = new ScrollView(ScrollViewMode.Vertical);
            if (toolkitManager.data.files != null)
            {
                foreach (var item in toolkitManager.data.files)
                {
                    scrollView.Add(FileVisualItem(item));
                }
            }

            scrollView.Add(getAllFilesButton);
            fileRequestContainer.Add(fileRequestField);
            fileRequestContainer.Add(fileRequestButton);

            panel.Add(fileRequestContainer);
            panel.Add(scrollView);

            return panel;

        }
        private VisualElement FileVisualItem(FigmaFileAsset file)
        {
            var item = new VisualElement();
            item.style.flexDirection = FlexDirection.Row;
            item.style.borderBottomWidth = 5;

            var buttons = new VisualElement();
            buttons.style.flexDirection = FlexDirection.Row;

            var labels = new VisualElement();
            labels.AddToClassList("panel75");

            var label = new Label() { text = file.name };
            label.style.overflow = Overflow.Hidden;
            label.style.textOverflow = TextOverflow.Ellipsis;
            label.style.unityTextOverflowPosition = TextOverflowPosition.Middle;
            label.style.borderLeftWidth = 2;

            var fileIDLabel = new Label() { text = file.fileId };
            fileIDLabel.style.fontSize = 8;
            fileIDLabel.style.borderLeftWidth = 2;

            var refreshButton = new Button() { text = "Refresh", name = "refreshbtn" };
            refreshButton.clicked += () => { toolkitManager.RefreshFile(file.fileId); };

            var webButton = new Button() { text = "View on web", name = "webbtn" };
            webButton.clicked += () => { Application.OpenURL($"https://www.figma.com/file/{file.fileId}"); };

            var deleteButton = new Button() { text = "Delete", name = "deletebtn" };
            deleteButton.clicked += () => { toolkitManager.DeleteLocalFile(file.fileId); RefreshFilesPanel(); };

            var loadButton = new Button() { text = "Load File", name = "loadbtn" };
            loadButton.clicked += () => { toolkitManager.GetDocument(file); ShowPanel(BuildDocumentPanel(file)); };
            loadButton.AddToClassList("panel25");

            labels.Add(label);
            labels.Add(fileIDLabel);
            labels.Add(buttons);

            buttons.Add(refreshButton);
            buttons.Add(webButton);
            buttons.Add(deleteButton);

            item.Add(labels);
            item.Add(loadButton);
            return item;
        }
        private void RefreshFilesPanel()
        {
            toolkitManager.GetLocalFiles();
            filesPanel.Clear();
            ShowPanel(filesPanel);
            BreadcrumbPop();
        }
    }
}