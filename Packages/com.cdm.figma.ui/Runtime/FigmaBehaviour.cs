using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cdm.Figma.UI
{
    [RequireComponent(typeof(FigmaNode))]
    public class FigmaBehaviour : UIBehaviour
    {
        private FigmaNode _attachedNode;
        public FigmaNode attachedNode => _attachedNode;

        protected override void Awake()
        {
            base.Awake();

            _attachedNode = GetComponent<FigmaNode>();

            var members = GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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

                        if (!string.IsNullOrEmpty(figmaNodeAttribute.bind))
                        {
                            bindingKey = figmaNodeAttribute.bind;
                        }

                        var fieldType = GetUnderlyingType(member);
                        //Debug.Log($"{member.Name}, {bindingKey}, {fieldType.Name}");

                        if (fieldType.IsSubclassOf(typeof(UnityEngine.Component)))
                        {
                            var target = attachedNode.Query(bindingKey, fieldType);
                            if (target != null)
                            {
                                SetMemberValue(member, this, target);
                            }
                        }
                        else
                        {
                            Debug.LogError(
                                $"{nameof(FigmaNodeAttribute)} only valid for types that sub class of UnityEngine.Component.");
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