using System.Collections;
using System.Collections.Generic;
using Cdm.Figma;
using UnityEngine;

public class FigmaFileTest : MonoBehaviour
{
    public string token;
    public string fileId;
    
    async void Start()
    {
        var figmaFile = await FigmaApi.GetFileAsync(new FigmaFileRequest(token, fileId));

        TraverseNodes(figmaFile.document);

    }

    private static void TraverseNodes(Node node)
    {
        Debug.Log($"{node.id} - {node.name} - {node.type}");
        
        var children = node.GetChildren();
        if (children != null)
        {
            foreach (var child in children)
            {
                TraverseNodes(child);
            }
        }
    }
}
