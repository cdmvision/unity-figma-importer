using Cdm.Figma.UI.Search;
using Cdm.Figma.UI.Utils;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Search;

namespace Cdm.Figma.UI
{
    [CustomPropertyDrawer(typeof(FigmaPage))]
    public class FigmaPagePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            const SearchViewFlags searchContextFlags = SearchViewFlags.None;

            var objType = typeof(FigmaPage);
            var context = CreateSearchContext();

            ObjectFieldUtils.DoObjectField(position, property, objType, label, context, searchContextFlags);
        }

        private static SearchContext CreateSearchContext()
        {
            return SearchService.CreateContext(new FigmaSearchProvider());
        }
    }
}