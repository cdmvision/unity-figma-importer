using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Cdm.Figma.UI.Utils
{
    public class FigmaNodeBinder
    {
        public static void Bind(object obj, FigmaNode node)
        {
            var members = obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var member in members)
            {
                if (member.MemberType.HasFlag(MemberTypes.Field) ||
                    member.MemberType.HasFlag(MemberTypes.Property))
                {
                    var figmaNodeAttribute = (FigmaNodeAttribute)member.GetCustomAttributes()
                        .FirstOrDefault(x => x.GetType() == typeof(FigmaNodeAttribute));
                    if (figmaNodeAttribute != null)
                    {
                        var bindingKey = member.Name;

                        if (!string.IsNullOrEmpty(figmaNodeAttribute.bindingKey))
                        {
                            bindingKey = figmaNodeAttribute.bindingKey;
                        }

                        var fieldType = GetUnderlyingType(member);
                        //Debug.Log($"{member.Name}, {bindingKey}, {fieldType.Name}");

                        if (typeof(UnityEngine.Component).IsAssignableFrom(fieldType))
                        {
                            var target = node.Query(bindingKey, fieldType);
                            if (target != null)
                            {
                                SetMemberValue(member, obj, target);
                            }
                        }
                        else
                        {
                            Debug.LogError(
                                $"{nameof(FigmaNodeAttribute)} only valid for types that sub class of {typeof(UnityEngine.Component)}. Got '{fieldType}'.");
                        }
                    }
                }
            }
        }
        
        private static Type GetUnderlyingType(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException(
                        "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo");
            }
        }

        private static void SetMemberValue(MemberInfo member, object target, object value)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)member).SetValue(target, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)member).SetValue(target, value, null);
                    break;
                default:
                    throw new ArgumentException("MemberInfo must be if type FieldInfo or PropertyInfo", "member");
            }
        }
    }
}