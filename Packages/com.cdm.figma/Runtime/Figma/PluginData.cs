using System;
using System.Runtime.Serialization;
using Cdm.Figma.Json;
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

        [DataMember]
        private string componentData { get; set; }

        [DataMember]
        public string tags { get; set; }

        public bool isIgnored { get; private set; }

        [DataMember(Name = "ignored")]
        private string ignored
        {
            get => isIgnored ? "true" : "false";
            set => isIgnored = value.ToLowerInvariant() == "true";
        }

        public bool hasBindingKey => !string.IsNullOrWhiteSpace(bindingKey);
        public bool hasLocalizationKey => !string.IsNullOrWhiteSpace(localizationKey);
        public bool hasComponentType => !string.IsNullOrWhiteSpace(componentType);
        public bool hasTags => !string.IsNullOrWhiteSpace(tags);

        public static PluginData FromString(string json)
        {
            return JsonHelper.Deserialize<PluginData>(json);
        }

        public static PluginData FromJson(JObject json)
        {
            return FromString(json.ToString());
        }

        public T GetComponentDataAs<T>() where T : class
        {
            try
            {
                if (!string.IsNullOrEmpty(componentData))
                {
                    return JsonHelper.Deserialize<T>(componentData);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }

        public override string ToString()
        {
            return JsonHelper.Serialize(this);
        }
    }
}