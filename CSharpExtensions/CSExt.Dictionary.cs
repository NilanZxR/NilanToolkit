using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace NilanToolkit.CSharpExtensions {
    public static partial class CSExt {

        public static TValue SetDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> factor) {
            if (!dict.TryGetValue(key, out var value)) {
                value = factor();
                dict[key] = value;
            }
            return value;
        }
        
        public static void ClearUnityObjectDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dict) where TValue : Object {
            foreach (var pair in dict) {
                Object.Destroy(pair.Value);
            }
            dict.Clear();
        }

        public static Dictionary<TKey, List<TValue>> GroupListBy<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector) {
            var dict = new Dictionary<TKey, List<TValue>>();
            foreach (var value in source) {
                var key = keySelector(value);
                var li = dict.SetDefault(key, () => new List<TValue>());
                li.Add(value);
            }
            return dict;
        }

    }
}
