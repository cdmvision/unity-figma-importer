using System;
using System.Collections.Generic;

namespace Cdm.Figma.UI
{
    public class FigmaAssetBinding : IEquatable<FigmaAssetBinding>
    {
        /// <summary>
        /// The type of the class.
        /// </summary>
        public Type type { get; }

        public ISet<FigmaAssetBindingMember> memberBindings { get; set;  }

        public FigmaAssetBinding(Type type)
        {
            this.type = type;
        }
        
        public FigmaAssetBinding(Type type, ISet<FigmaAssetBindingMember> memberBindings)
        {
            this.type = type;
            this.memberBindings = memberBindings;
        }

        public bool Equals(FigmaAssetBinding other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return type == other.type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FigmaAssetBinding)obj);
        }

        public override int GetHashCode()
        {
            return (type != null ? type.GetHashCode() : 0);
        }

        public static bool operator ==(FigmaAssetBinding left, FigmaAssetBinding right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FigmaAssetBinding left, FigmaAssetBinding right)
        {
            return !Equals(left, right);
        }
    }
}