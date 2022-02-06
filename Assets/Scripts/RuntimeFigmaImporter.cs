using System;
using Cdm.Figma.UI;
using UnityEngine;

public class RuntimeFigmaImporter : MonoBehaviour
{
    public FigmaFile figmaFile;
    public Canvas canvas;
    
    private async void Start()
    {
        try
        {
            var figmaImporter = new FigmaImporter();
            await figmaImporter.ImportFileAsync(figmaFile);

            var documents = figmaImporter.GetImportedDocuments();
            foreach (var document in documents)
            {
                document.nodeObject.rectTransform.SetParent(canvas.transform, false);
            }
            
            Debug.Log($"Figma file has been imported successfully: {figmaFile.name} ({figmaFile.id})");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
