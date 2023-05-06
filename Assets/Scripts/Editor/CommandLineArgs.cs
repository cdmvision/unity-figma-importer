using System.Linq;
using UnityEditor;
using UnityEngine;

public class CommandLineArgs
{
    [InitializeOnLoadMethod]
    public static void PrintArgs()
    {
        // Print command line arguments for detection of error in Unity cloud build.
        var args = string.Join(" ", System.Environment.GetCommandLineArgs());
        Debug.Log($"Editor args: {args}");
    }
}