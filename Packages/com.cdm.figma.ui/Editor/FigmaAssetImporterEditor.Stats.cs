using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
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
            LabelField("Rectangle Texture Size", assetImporter.rectTextureSize.ToString());

            EditorGUILayout.Space();

            LabelField("Page Count", _stats.pageCount.ToString());
            LabelField("Node Count", _stats.nodeCount.ToString());
            LabelField("Sprite Count", _stats.spriteCount.ToString());
            LabelField("Texture Count", _stats.textureCount.ToString());
            LabelField("Total Runtime Texture Memory", GetHumanReadableMemorySize(_stats.totalTextureMemorySize));

            EditorGUILayout.Space();

            LabelField("Texture Count - Smallest", $"{_stats.smallestTextureCount} (<= {minTextureSize})");
            LabelField("Texture Count - Small", $"{_stats.smallTextureCount} (< {avgTextureSize})");
            LabelField("Texture Count - Big", $"{_stats.bigTextureCount} (>= {avgTextureSize})");
            LabelField("Texture Count - Biggest", $"{_stats.biggestTextureCount} (>= {maxTextureSize})");
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

            _stats.smallestTextureCount = CountTexture(assets, (t, min, max) => t.width <= min || t.height <= min);
            _stats.biggestTextureCount = CountTexture(assets, (t, min, max) => t.width >= max || t.height >= max);

            foreach (var asset in assets)
            {
                if (asset is Texture texture)
                {
                    _stats.totalTextureMemorySize += Profiler.GetRuntimeMemorySizeLong(texture);
                }
            }
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

        /// <summary>
        /// Returns x Bytes, kB, Mb, etc... 
        /// </summary>
        /// <seealso cref="https://stackoverflow.com/a/48707695/3261507"/>
        public static string GetHumanReadableMemorySize(long value)
        {
            var thresholds = new KeyValuePair<long, string>[]
            {
                new KeyValuePair<long, string>(1, " Byte"),
                new KeyValuePair<long, string>(2, " Bytes"),
                new KeyValuePair<long, string>(1024, " KB"),
                new KeyValuePair<long, string>(1048576, " MB"), // Note: 1024 ^ 2 = 1026 (xor operator)
                new KeyValuePair<long, string>(1073741824, " GB"),
                new KeyValuePair<long, string>(1099511627776, " TB"),
                new KeyValuePair<long, string>(1125899906842620, " PB"),
                new KeyValuePair<long, string>(1152921504606850000, " EB"),
            };

            if (value == 0)
                return "0 Bytes";

            for (var t = thresholds.Length - 1; t > 0; t--)
            {
                if (value >= thresholds[t].Key)
                {
                    return ((double)value / thresholds[t].Key).ToString("0.00") + thresholds[t].Value;
                }
            }

            // negative bytes (common case optimised to the end of this routine)
            return "-" + GetHumanReadableMemorySize(-value);
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
            public long totalTextureMemorySize;
        }
    }
}