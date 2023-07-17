using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// The line node represents a one-dimensional object. It is typically given a stroke so that it is visible. Its
    /// two primary properties are its length and rotation. In Figma and in our API, this is represented by the
    /// width and rotation. The height of a line is always 0.
    /// </summary>
    [DataContract]
    public class LineNode : VectorNode
    {
        public override string type => NodeType.Line;
    }
}