using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ComponentProperties
    {
        /// <summary>
        /// It is available only in <see cref="ComponentNode"/> and <see cref="ComponentSetNode"/>.
        /// </summary>
        [DataMember(Name = "definitions")]
        public Dictionary<string, ComponentPropertyDefinition> definitions { get; private set; } =
            new Dictionary<string, ComponentPropertyDefinition>();
        
        /// <summary>
        /// It is available only in <see cref="InstanceNode"/>.
        /// </summary>
        [DataMember(Name = "assignments")]
        public Dictionary<string, ComponentPropertyAssignment> assignments { get; private set; } =
            new Dictionary<string, ComponentPropertyAssignment>();

        /// <summary>
        /// It is available only in <see cref="InstanceNode"/> and if it is located in a <see cref="ComponentNode"/>.
        /// </summary>
        [DataMember(Name = "references")]
        public ComponentPropertyReferences references { get; private set; }
    }
}