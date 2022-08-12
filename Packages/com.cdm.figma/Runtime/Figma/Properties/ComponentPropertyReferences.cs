using System.Runtime.Serialization;

namespace Cdm.Figma
{
    [DataContract]
    public class ComponentPropertyReferences
    {
        /// <summary>
        /// Available only for <see cref="ComponentPropertyType.InstanceSwap"/>.
        /// Its value is a key in <see cref="ComponentProperties.definitions"/> dictionary.
        /// </summary>
        [DataMember]
        public string mainComponent { get; set; }

        /// <summary>
        /// Available only for <see cref="ComponentPropertyType.Text"/>.
        /// Its value is a key in <see cref="ComponentProperties.definitions"/> dictionary.
        /// </summary>
        [DataMember]
        public string characters { get; set; }

        /// <summary>
        /// Available only for <see cref="ComponentPropertyType.Boolean"/>.
        /// Its value is a key in <see cref="ComponentProperties.definitions"/> dictionary.
        /// </summary>
        [DataMember]
        public string visible { get; set; }
    }
}