using System;
using System.Reflection;

namespace Cdm.Figma.UI
{
    public class FigmaAssetBindingMember : IEquatable<FigmaAssetBindingMember>
    {
        /// <summary>
        /// The member that is going to be bound with given <see cref="asset"/>.
        /// </summary>
        public MemberInfo member { get; }

        /// <summary>
        /// The asset is going to be bound with <see cref="member"/>.
        /// </summary>
        public Object asset { get; }

        public FigmaAssetBindingMember(MemberInfo member, Object asset)
        {
            this.member = member;
            this.asset = asset;
        }

        public bool Equals(FigmaAssetBindingMember other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(member, other.member);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FigmaAssetBindingMember)obj);
        }

        public override int GetHashCode()
        {
            return (member != null ? member.GetHashCode() : 0);
        }

        public static bool operator ==(FigmaAssetBindingMember left, FigmaAssetBindingMember right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FigmaAssetBindingMember left, FigmaAssetBindingMember right)
        {
            return !Equals(left, right);
        }
    }
}