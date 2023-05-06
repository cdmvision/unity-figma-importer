using System.Linq;
using UnityEditor;
using UnityEngine;

public class CommandLineArgs
{
    [InitializeOnLoadMethod]
    public static void PrintArgs()
    {
        var args = string.Join(" ", System.Environment.GetCommandLineArgs());
        Debug.Log($"Editor args: {args}");
    }
}