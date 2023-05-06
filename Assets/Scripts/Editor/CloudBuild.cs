using UnityEditor;
using UnityEngine;

public class CloudBuild
{
    [InitializeOnLoadMethod]
    public static void PreExport()
    {
        // Print command line arguments for detection of error in Unity cloud build.
        var args = string.Join(" ", System.Environment.GetCommandLineArgs());
        Debug.Log($"Editor args: {args}");
    }
}