using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cdm.Figma.UI
{
    public class AssetCache : IEnumerable<KeyValuePair<string, Object>>
    {
        private readonly Dictionary<string, Object> _assets = new Dictionary<string, Object>();

        public bool Add<T>(string key, T asset) where T : Object
        {
            return _assets.TryAdd(GetKeyWithType<T>(key), asset);
        }

        public bool Remove<T>(string key) where T : Object
        {
            return _assets.Remove(GetKeyWithType<T>(key));
        }

        public bool Contains<T>(string key) where T : Object
        {
            return _assets.ContainsKey(GetKeyWithType<T>(key));
        }

        public bool TryGet<T>(string key, out T value) where T : Object
        {
            if (_assets.TryGetValue(GetKeyWithType<T>(key), out var obj))
            {
                value = (T)obj;
                return true;
            }

            value = null;
            return false;
        }

        public void Clear()
        {
            _assets.Clear();
        }

        private static string GetKeyWithType<T>(string key)
        {
            return $"{key}_{typeof(T).Name}";
        }

        public IEnumerator<KeyValuePair<string, Object>> GetEnumerator()
        {
            return _assets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _assets.GetEnumerator();
        }
    }
}