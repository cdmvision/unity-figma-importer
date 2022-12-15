using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

namespace Cdm.Figma.UI.Editor.Search
{
    public class FigmaSearchProvider : SearchProvider
    {
        private const string DisplayName = "Figma";
        private const string ProviderId = "figma";

        public FigmaSearchProvider() : base(ProviderId, DisplayName)
        {
            fetchItems = (context, items, provider) => SearchItems(context, provider);
            toObject = (item, type) => (FigmaPage)item.data;
            isExplicitProvider = true;
        }
        
        private static IEnumerator<SearchItem> SearchItems(SearchContext context, SearchProvider provider)
        {
            var matches = new List<int>();

            foreach (var (identifier, figmaDesign, figmaPage) in GetFigmaPages())
            {
                var score = 0L;
                matches.Clear();

                if (FuzzySearch.FuzzyMatch(context.searchQuery, figmaPage.name, ref score, matches))
                {
                    var label = figmaPage.name;
                    var description = $"{figmaDesign.name} ({figmaDesign.id})";
                    var thumbnail = figmaDesign.thumbnail;
                    
                    yield return provider.CreateItem(
                        context, identifier, (int)score, label, description, thumbnail, figmaPage);
                }
                else
                {
                    yield return null;
                }
            }
        }

        private static IEnumerable<Tuple<string, FigmaDesign, FigmaPage>> GetFigmaPages()
        {
            var pages = new List<Tuple<string, FigmaDesign, FigmaPage>>();
            var assets = AssetDatabase.FindAssets($"t:{nameof(GameObject)}");

            foreach (var figmaDesignGuid in assets)
            {
                var figmaDesignPath = AssetDatabase.GUIDToAssetPath(figmaDesignGuid);
                var figmaDesignGo = AssetDatabase.LoadAssetAtPath<GameObject>(figmaDesignPath);
                if (figmaDesignGo != null)
                {
                    var figmaDesign = figmaDesignGo.GetComponent<FigmaDesign>();
                    if (figmaDesign != null)
                    {
                        foreach (var page in figmaDesign.document.pages)
                        {
                            var identifier = $"{figmaDesignGuid}/{page.nodeId}";
                            pages.Add(
                                new Tuple<string, FigmaDesign, FigmaPage>(identifier, figmaDesign, page));
                        }
                    }
                }
            }

            return pages;
        }

        [SearchItemProvider]
        internal static SearchProvider CreateProvider()
        {
            return new FigmaSearchProvider();
        }
    }
}