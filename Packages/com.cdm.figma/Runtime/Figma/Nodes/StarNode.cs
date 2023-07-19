using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// The star node allows you to create a star with a set number of points.
    /// </summary>
    [DataContract]
    public class StarNode : VectorNode
    {
        public override NodeType type => NodeType.Star;
    }
}