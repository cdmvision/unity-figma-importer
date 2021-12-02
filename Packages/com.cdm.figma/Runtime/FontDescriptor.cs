using System;
using UnityEngine;

namespace Cdm.Figma
{
    [Serializable]
    public class FontDescriptor : IEquatable<FontDescriptor>
    {
        [SerializeField]
        private string _family;
        
        public string family
        {
            get => _family;
            set => _family = value;
        }
        
        [SerializeField]
        private FontWeight _weight;
        
        public FontWeight weight
        {
            get => _weight;
            set => _weight = value;
        }
        
        [SerializeField]
        private bool _italic;
        
        public bool italic
        {
            get => _italic;
            set => _italic = value;
        }

        public FontDescriptor()
        {
        }

        public FontDescriptor(string family, FontWeight weight = FontWeight.Regular, bool italic = false)
        {
            this.family = family;
            this.weight = weight;
            this.italic = italic;
        }

        public bool Equals(FontDescriptor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _family == other._family && _weight == other._weight && _italic == other._italic;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FontDescriptor) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_family, (int) _weight, _italic);
        }

        public static bool operator ==(FontDescriptor left, FontDescriptor right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FontDescriptor left, FontDescriptor right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{family}-{weight}{(italic ? "-Italic" : "")}";
        }
    }
}