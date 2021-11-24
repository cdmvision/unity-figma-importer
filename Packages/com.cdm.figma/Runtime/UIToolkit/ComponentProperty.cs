using System;

namespace Cdm.Figma.UIToolkit
{
    [Serializable]
    public class ComponentProperty : IEquatable<ComponentProperty>
    {
        public string key;
        public ComponentVariant[] variants = new ComponentVariant[0];

        public bool Equals(ComponentProperty other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return key == other.key;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ComponentProperty) obj);
        }

        public override int GetHashCode()
        {
            return (key != null ? key.GetHashCode() : 0);
        }

        public static bool operator ==(ComponentProperty left, ComponentProperty right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ComponentProperty left, ComponentProperty right)
        {
            return !Equals(left, right);
        }
    }
    
    [Serializable]
    public class ComponentVariant
    {
        public string key;
        public string value;

        public ComponentVariant()
        {
        }
        
        public ComponentVariant(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
}