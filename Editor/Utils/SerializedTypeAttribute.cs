using System;
using UnityEngine;

namespace Cdm.Figma.UI.Editor.Utils
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializedTypeAttribute : PropertyAttribute
    {
        public string baseType { get; }

        public SerializedTypeAttribute(Type baseType)
        {
            this.baseType = baseType.AssemblyQualifiedName;
        }
    }
}