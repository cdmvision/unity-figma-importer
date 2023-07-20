using System.Runtime.Serialization;
using Cdm.Figma.Json;
using Newtonsoft.Json;

namespace Cdm.Figma.UI
{
    [DataContract]
    public class InputFieldComponentData
    {
        [DataMember]
        [JsonConverter(typeof(ColorHexJsonConverter))]
        public Color selectionColor { get; set; }

        [DataMember]
        public int selectionColorOpacity { get; set; } = 75;
        
        [DataMember]
        [JsonConverter(typeof(ColorHexJsonConverter))]
        public Color caretColor { get; set; }

        [DataMember]
        public int caretColorOpacity { get; set; } = 100;

        [DataMember]
        public int caretWidth { get; set; } = 1;
    }
}