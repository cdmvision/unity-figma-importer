using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cdm.Figma
{
    [DataContract]
    public class PluginData
    {
        public const string Id = "1047282855279327962";
        
        [DataMember]
        public string bindingKey { get; set; }
        
        [DataMember]
        public string localizationKey { get; set; }
        
        [DataMember]
        public string componentType { get; set; }

        public bool hasBindingKey => !string.IsNullOrEmpty(bindingKey);
        public bool hasLocalizationKey => !string.IsNullOrEmpty(localizationKey);
        public bool hasComponentType => !string.IsNullOrEmpty(componentType);

        public static PluginData FromString(string json)
        {
            return JsonConvert.DeserializeObject<PluginData>(json, JsonSerializerHelper.Settings);
        }

        public static PluginData FromJson(JObject json)
        {
            return FromString(json.ToString());
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, JsonSerializerHelper.Settings);
        }
    }
}