using System;
using UnityEngine;

namespace Cdm.Figma
{
    /// <summary>
    /// A class to hold the file response from a figma API call,
    /// additionally populated with the name and id of the file.
    /// </summary>
    [Serializable]
    public class FigmaFileAsset
    {
        /// <summary>
        /// ID of the Figma document.
        /// </summary>
        public string fileId;
        
        /// <summary>
        /// Name of the Figma file.
        /// </summary>
        public string name;
        
        /// <summary>
        /// Figma file that is response from a Figma API call.
        /// </summary>
        public TextAsset file;
    }
}