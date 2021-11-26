using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class CanvasNode : Node
    {
        public override string type => NodeType.Canvas;
        
        /// <summary>
        /// A list of top level layers on the canvas.
        /// </summary>
        [DataMember(Name = "children")]
        public Node[] children { get; set; }
        
        /// <summary>
        /// Background color of the canvas.
        /// </summary>
        [DataMember(Name = "backgroundColor")]
        public Color backgroundColor { get; set; }

        /// <summary>
        /// A array of flow starting points sorted by its position in the prototype settings panel.
        /// </summary>
        [DataMember(Name = "flowStartingPoints")]
        public List<FlowStartingPoint> flowStartingPoints { get; private set; } = new List<FlowStartingPoint>();
        
        /// <summary>
        /// A list of export settings representing images to export from the canvas.
        /// </summary>
        [DataMember(Name = "exportSettings")]
        public List<ExportSetting> exportSettings { get; private set; } = new List<ExportSetting>();

        public override Node[] GetChildren() => children;
    }

    /// <summary>
    /// A flow starting point used when launching a prototype to enter Presentation view.
    /// </summary>
    [DataContract]
    public class FlowStartingPoint
    {
        /// <summary>
        /// Unique identifier specifying the frame.
        /// </summary>
        [DataMember(Name = "nodeId")]
        public string nodeId { get; set; }
        
        /// <summary>
        /// Unique identifier specifying the frame.
        /// </summary>
        [DataMember(Name = "name")]
        public string name { get; set; }
    }
}