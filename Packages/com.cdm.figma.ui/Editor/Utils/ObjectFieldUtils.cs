using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Search;

namespace Cdm.Figma.UI.Utils
{
    public static class ObjectFieldUtils
    {
#if UNITY_2022_2_OR_NEWER
        internal static void DoObjectField(Rect position, SerializedProperty property, Type objectType, GUIContent label,
            SearchContext searchContext, SearchViewFlags searchContextFlags)
        {
            UnityEditor.Search.ObjectField.DoObjectField(
                position, property, objectType, label, searchContext, searchContextFlags);
        }
#else
        private static MethodInfo _doObjectFieldMethod;

        internal static void DoObjectField(Rect position, SerializedProperty property, Type objectType, GUIContent label,
            SearchContext searchContext, SearchViewFlags searchContextFlags)
        {
            if (_doObjectFieldMethod == null)
            {
                var assembly =
                    Assembly.Load(
                        "UnityEditor.QuickSearchModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
                var type = assembly.GetType("UnityEditor.Search.ObjectField");

                _doObjectFieldMethod = type.GetMethod("DoObjectField", new[]
                {
                    typeof(Rect), typeof(SerializedProperty), typeof(Type), typeof(GUIContent), typeof(SearchContext),
                    typeof(SearchViewFlags)
                });
            }

            _doObjectFieldMethod?.Invoke(null, new object[]
            {
                position, property, objectType, label, searchContext, searchContextFlags
            });
        }


#endif
    }
}