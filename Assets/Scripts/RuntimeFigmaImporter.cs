using System;
using System.Diagnostics;
using Cdm.Figma;
using Cdm.Figma.UI;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RuntimeFigmaImporter : MonoBehaviour
{
    public TextAsset figmaFile;
    public Canvas canvas;

    private void Start()
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var file = FigmaFile.FromString(figmaFile.text);
            
            var figmaImporter = new FigmaImporter();
            figmaImporter.ImportFile(file);

            var documents = figmaImporter.GetImportedDocuments();
            foreach (var document in documents)
            {
                document.nodeObject.rectTransform.SetParent(canvas.transform, false);
            }

            stopwatch.Stop();

            Debug.Log(
                $"Figma file has been imported successfully in {stopwatch.ElapsedMilliseconds} ms: {file.name} ({file.fileID})");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}