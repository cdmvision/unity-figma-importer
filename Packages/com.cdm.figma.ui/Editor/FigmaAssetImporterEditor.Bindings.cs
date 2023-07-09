using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cdm.Figma.UI.Utils;
using UnityEditor;
using UnityEngine;

namespace Cdm.Figma.UI.Editor
{
    public partial class FigmaAssetImporterEditor
    {
        private const string AssetBindingTypePropertyPath =
            nameof(FigmaAssetImporter.SerializableAssetBinding.type);

        private const string AssetBindingsPropertyPath =
            nameof(FigmaAssetImporter.SerializableAssetBinding.bindings);

        private const string AssetBindingNamePropertyPath =
            nameof(FigmaAssetImporter.SerializableAssetBindingMember.name);

        private const string AssetBindingAssetPropertyPath =
            nameof(FigmaAssetImporter.SerializableAssetBindingMember.asset);

        private Type[] _bindingTypes;
        private List<AssetBindingMember>[] _bindingMembers;
        private bool[] _bindingFoldouts;

        private void DrawBindingsGui(bool refresh)
        {
            if (refresh)
            {
                RefreshAssetBindings();
            }
            
            for (var i = 0; i < _bindingTypes.Length; i++)
            {
                var bindingType = _bindingTypes[i];
                
                _bindingFoldouts[i] = EditorGUILayout.Foldout(_bindingFoldouts[i], bindingType.FullName);
                if (!_bindingFoldouts[i])
                    continue;

                EditorGUI.indentLevel += 1;

                foreach (var memberBinding in _bindingMembers[i])
                {
                    var bindingIndex = -1;
                    var member = memberBinding.member;
                    var attribute = memberBinding.attribute;
                    UnityEngine.Object value = null;

                    var bindingTypeIndex = FindSerializableAssetBindingByType(bindingType);
                    if (bindingTypeIndex >= 0)
                    {
                        bindingIndex = FindSerializableAssetBindingMemberByType(bindingTypeIndex, member);

                        if (bindingIndex >= 0)
                        {
                            var bindingProperty = _assetBindings.GetArrayElementAtIndex(bindingTypeIndex);
                            var bindingsProperty = bindingProperty.FindPropertyRelative(AssetBindingsPropertyPath);
                            var memberProperty = bindingsProperty.GetArrayElementAtIndex(bindingIndex);
                            var assetProperty = memberProperty.FindPropertyRelative(AssetBindingAssetPropertyPath);
                            value = assetProperty.objectReferenceValue;
                        }
                    }

                    var displayName = !string.IsNullOrEmpty(attribute.name)
                        ? attribute.name
                        : ObjectNames.NicifyVariableName(member.Name);

                    var fieldType = ReflectionHelper.GetUnderlyingType(member);

                    EditorGUI.BeginChangeCheck();
                    var newValue = EditorGUILayout.ObjectField(
                        new GUIContent(displayName, member.Name), value, fieldType, false);

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (newValue != null)
                        {
                            if (bindingTypeIndex < 0)
                            {
                                _assetBindings.arraySize += 1;
                                bindingTypeIndex = _assetBindings.arraySize - 1;
                            }

                            var bindingProperty = _assetBindings.GetArrayElementAtIndex(bindingTypeIndex);
                            var bindingTypeProperty =
                                bindingProperty.FindPropertyRelative(AssetBindingTypePropertyPath);
                            bindingTypeProperty.stringValue = bindingType.AssemblyQualifiedName;

                            var bindingsProperty = bindingProperty.FindPropertyRelative(AssetBindingsPropertyPath);

                            if (bindingIndex < 0)
                            {
                                bindingsProperty.arraySize += 1;
                                bindingIndex = bindingsProperty.arraySize - 1;
                            }

                            var memberProperty = bindingsProperty.GetArrayElementAtIndex(bindingIndex);
                            var nameProperty = memberProperty.FindPropertyRelative(AssetBindingNamePropertyPath);
                            var assetProperty = memberProperty.FindPropertyRelative(AssetBindingAssetPropertyPath);

                            nameProperty.stringValue = member.Name;
                            assetProperty.objectReferenceValue = newValue;
                        }
                        else
                        {
                            if (bindingTypeIndex >= 0)
                            {
                                var bindingProperty = _assetBindings.GetArrayElementAtIndex(bindingTypeIndex);
                                var bindingsProperty = bindingProperty.FindPropertyRelative(AssetBindingsPropertyPath);

                                if (bindingIndex >= 0)
                                {
                                    bindingsProperty.DeleteArrayElementAtIndex(bindingIndex);
                                }

                                if (bindingsProperty.arraySize == 0)
                                {
                                    _assetBindings.DeleteArrayElementAtIndex(bindingTypeIndex);
                                }
                            }
                        }
                    }
                }

                EditorGUI.indentLevel -= 1;
            }
        }

        private void RefreshAssetBindings()
        {
            _bindingTypes = null;
            _bindingMembers = null;
            _bindingFoldouts = null;

            var monoBehaviours = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => typeof(MonoBehaviour).IsAssignableFrom(t)));

            var bindings = new Dictionary<Type, List<AssetBindingMember>>();

            foreach (var monoBehaviour in monoBehaviours)
            {
                var members = monoBehaviour.GetMembers(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var member in members)
                {
                    if (!member.MemberType.HasFlag(MemberTypes.Field) &&
                        !member.MemberType.HasFlag(MemberTypes.Property))
                        continue;

                    var figmaAssetAttribute = (FigmaAssetAttribute)member.GetCustomAttributes()
                        .FirstOrDefault(x => x.GetType() == typeof(FigmaAssetAttribute));

                    if (figmaAssetAttribute == null)
                        continue;

                    if (!bindings.ContainsKey(monoBehaviour))
                    {
                        bindings.Add(monoBehaviour, new List<AssetBindingMember>());
                    }

                    bindings[monoBehaviour].Add(new AssetBindingMember(member, figmaAssetAttribute));
                }
            }

            _bindingTypes = bindings.Keys.ToArray();
            _bindingMembers = bindings.Values.ToArray();
            _bindingFoldouts = new bool[_bindingTypes.Length];
        }

        private int FindSerializableAssetBindingByType(Type targetType)
        {
            if (targetType == null)
                return -1;

            for (var i = 0; i < _assetBindings.arraySize; i++)
            {
                var binding = _assetBindings.GetArrayElementAtIndex(i);
                var bindingTypeName = binding.FindPropertyRelative(AssetBindingTypePropertyPath)?.stringValue;

                var type = Type.GetType(bindingTypeName ?? "");
                if (type == targetType)
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindSerializableAssetBindingMemberByType(int bindingIndex, MemberInfo memberInfo)
        {
            const string bindingsPropertyPath = nameof(FigmaAssetImporter.SerializableAssetBinding.bindings);
            const string namePropertyPath = nameof(FigmaAssetImporter.SerializableAssetBindingMember.name);

            var binding = _assetBindings.GetArrayElementAtIndex(bindingIndex);
            var members = binding.FindPropertyRelative(bindingsPropertyPath);

            for (var i = 0; i < members.arraySize; i++)
            {
                var member = members.GetArrayElementAtIndex(i);
                var memberName = member.FindPropertyRelative(namePropertyPath);

                if (memberName.stringValue.Equals(memberInfo.Name))
                {
                    return i;
                }
            }

            return -1;
        }

        private readonly struct AssetBindingMember
        {
            public MemberInfo member { get; }
            public FigmaAssetAttribute attribute { get; }

            public AssetBindingMember(MemberInfo member, FigmaAssetAttribute attribute)
            {
                this.member = member;
                this.attribute = attribute;
            }
        }
    }
}