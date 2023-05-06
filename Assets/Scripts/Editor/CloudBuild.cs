using UnityEngine;

namespace Cdm
{
    public class CloudBuild
    {
        public static void PreExport(UnityEngine.CloudBuild.BuildManifestObject manifest)
        {
            // Print command line arguments for detection of error in Unity cloud build.
            var args = string.Join(" ", System.Environment.GetCommandLineArgs());
            Debug.Log($"Editor args: {args}");
        }
    }
}
