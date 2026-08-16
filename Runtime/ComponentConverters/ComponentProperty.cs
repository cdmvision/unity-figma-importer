namespace Cdm.Figma.UI
{
    public readonly struct ComponentProperty
    {
        public string key { get; }
        public string value { get; }

        public ComponentProperty(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public override string ToString()
        {
            return $"{key}={value}";
        }
    }
}