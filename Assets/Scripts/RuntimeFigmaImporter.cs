using System;
using System.Diagnostics;
using Cdm.Figma.UI;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RuntimeFigmaImporter : MonoBehaviour
{
    public FigmaFile figmaFile;
    public Canvas canvas;
    
    private async void Start()
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            
            var figmaImporter = new FigmaImporter();
            await figmaImporter.ImportFileAsync(figmaFile);

            var documents = figmaImporter.GetImportedDocuments();
            foreach (var document in documents)
            {
                document.nodeObject.rectTransform.SetParent(canvas.transform, false);
            }
            
            stopwatch.Stop();
            
            Debug.Log($"Figma file has been imported successfully in {stopwatch.ElapsedMilliseconds} ms: {figmaFile.name} ({figmaFile.id})");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
