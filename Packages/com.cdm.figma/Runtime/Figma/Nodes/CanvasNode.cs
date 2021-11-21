using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cdm.Figma
{
    [Serializable]
    public class CanvasNode : Node
    {
        public override NodeType type => NodeType.Canvas;
        
        /// <summary>
        /// A list of top level layers on the canvas.
        /// </summary>
        [JsonProperty("children")]
        public Node[] children { get; set; }
        
        /// <summary>
        /// Background color of the canvas.
        /// </summary>
        [JsonProperty("backgroundColor")]
        public Color backgroundColor { get; set; }

        /// <summary>
        /// A array of flow starting points sorted by its position in the prototype settings panel.
        /// </summary>
        [JsonProperty("flowStartingPoints")]
        public List<FlowStartingPoint> flowStartingPoints { get; private set; } = new List<FlowStartingPoint>();
        
        /// <summary>
        /// A list of export settings representing images to export from the canvas.
        /// </summary>
        [JsonProperty("exportSettings")]
        public List<ExportSetting> exportSettings { get; private set; } = new List<ExportSetting>();

        public override Node[] GetChildren() => children;
    }

    /// <summary>
    /// A flow starting point used when launching a prototype to enter Presentation view.
    /// </summary>
    [Serializable]
    public class FlowStartingPoint
    {
        /// <summary>
        /// Unique identifier specifying the frame.
        /// </summary>
        [JsonProperty("nodeId")]
        public string nodeId { get; set; }
        
        /// <summary>
        /// Unique identifier specifying the frame.
        /// </summary>
        [JsonProperty("name")]
        public string name { get; set; }
    }
}