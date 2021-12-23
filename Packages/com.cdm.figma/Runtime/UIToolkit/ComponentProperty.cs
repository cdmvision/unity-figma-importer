using System;

namespace Cdm.Figma.UIToolkit
{
    public class ComponentProperty : IEquatable<ComponentProperty>
    {
        public string key { get; }
        public string[] variants { get; }

        public ComponentProperty(string key, string[] variants)
        {
            this.key = key;
            this.variants = variants;
        }

        public string ToString(int variantIndex)
        {
            if (variantIndex < 0 || variantIndex >= variants.Length)
                throw new ArgumentOutOfRangeException(nameof(variantIndex), variantIndex, "Variant index out of bounds.");

            return $"{key}={variants[variantIndex]}";
        }

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
}