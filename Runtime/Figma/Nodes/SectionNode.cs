using System.Runtime.Serialization;

namespace Cdm.Figma
{
    /// <summary>
    /// Sections are used to organize related objects.
    /// </summary>
    [DataContract]
    public class SectionNode : SceneNode
    {
        [DataMember(Name = "sectionContentsHidden")]
        public bool sectionContentsHidden { get; set; }
    }
}