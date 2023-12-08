using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cdm.Figma.UI.Editor
{
    public partial class FigmaAssetImporterEditor
    {
        private Stats _stats;

        private void DrawStatsGui()
        {
            var assetImporter = (FigmaAssetImporter)target;
            
            var minTextureSize = assetImporter.minTextureSize;
            var maxTextureSize = assetImporter.maxTextureSize;
            var avgTextureSize = assetImporter.maxTextureSize / 2;
            
            LabelField("Min Texture Size", minTextureSize.ToString());
            LabelField("Max Texture Size", maxTextureSize.ToString());
            
            EditorGUILayout.Space();
            
            LabelField("Page Count", _stats.pageCount.ToString());
            LabelField("Node Count", _stats.nodeCount.ToString());
            LabelField("Sprite Count", _stats.spriteCount.ToString());
            LabelField("Texture Count", _stats.textureCount.ToString());

            EditorGUILayout.Space();
            
            LabelField("Texture Count - Smallest", $"{_stats.smallestTextureCount} (= {minTextureSize})");
            LabelField("Texture Count - Small", $"{_stats.smallTextureCount} (< {avgTextureSize})");
            LabelField("Texture Count - Big", $"{_stats.bigTextureCount} (>= {avgTextureSize})");
            LabelField("Texture Count - Biggest", $"{_stats.biggestTextureCount} (= {maxTextureSize})");
        }

        private void InitStats()
        {
            var assetImporter = (FigmaAssetImporter)target;
            var assetPath = AssetDatabase.GetAssetPath(target);

            _stats = new Stats();

            var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            _stats.pageCount = CountComponent<FigmaPage>(assets);
            _stats.nodeCount = CountComponent<FigmaNode>(assets);
            _stats.spriteCount = assets.Count(x => x is Sprite);
            _stats.textureCount = assets.Count(x => x is Texture);

            _stats.bigTextureCount = CountTexture(assets, (t, min, max) =>
            {
                var size = max / 2;
                return t.width >= size || t.height >= size;
            });

            _stats.smallTextureCount = _stats.textureCount - _stats.bigTextureCount;

            _stats.smallestTextureCount = CountTexture(assets, (t, min, max) => t.width == min || t.height == min);
            _stats.biggestTextureCount = CountTexture(assets, (t, min, max) => t.width == max || t.height == max);
        }

        private int CountTexture(IEnumerable<Object> assets, Func<Texture, int, int, bool> predicate)
        {
            var assetImporter = (FigmaAssetImporter)target;
            var minTextureSize = assetImporter.minTextureSize;
            var maxTextureSize = assetImporter.maxTextureSize;

            return assets.Count(x =>
            {
                if (x is Texture texture)
                {
                    return predicate(texture, minTextureSize, maxTextureSize);
                }

                return false;
            });
        }

        private static int CountComponent<T>(IEnumerable<Object> assets) where T : UnityEngine.Component
        {
            return assets.Count(x =>
            {
                if (x is GameObject go)
                {
                    return go.GetComponent<T>() != null;
                }

                return false;
            });
        }

        private class Stats
        {
            public int pageCount;
            public int nodeCount;
            public int spriteCount;
            public int textureCount;
            public int smallTextureCount;
            public int smallestTextureCount;
            public int bigTextureCount;
            public int biggestTextureCount;
        }
    }
}