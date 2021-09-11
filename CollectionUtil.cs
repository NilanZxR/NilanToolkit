using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NilanToolkit {
public static class CollectionUtil {
    
    /// <summary>
    /// like linq.select, but only select item not null
    /// </summary>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TRet"></typeparam>
    /// <returns></returns>
    public static IEnumerable<TRet> SelectNotNull<TSource,TRet>(this IEnumerable<TSource> source, Func<TSource,TRet> selector) 
    where TRet : class {
        var list = new List<TRet>();
        foreach (var item in source) {
            var selected = selector(item);
            if (selected == null) continue;
            list.Add(selected);
        }
        return list;
    }
    
    public static bool IsInRange(this IList list, int index) {
        return index >= 0 && index < list.Count;
    }

    public static bool IsInRange(this Array arr, int index) {
        return index >= 0 && index < arr.Length;
    }
    
    public static T[] Transit<T>(this T[] arr, int delta) {
        delta = delta % arr.Length;
        if (delta < 0) delta = arr.Length + delta;
        var ret = new T[arr.Length];
        for (int i = 0; i < ret.Length; i++) {
            ret[i] = arr[(i + delta) % arr.Length];
        }

        return ret;
    }

    public static bool TryFindItem<T>(this IEnumerable<T> list, Func<T, bool> predicate, out T item) {
        foreach (var t in list) {
            if (predicate(t)) {
                item = t;
                return true;
            }
        }
        item = default;
        return false;
    }

    public static void ClearUnityObjectList<T>(this IList<T> list) where T : Object{
        foreach (T t in list) {
            if (t is MonoBehaviour behaviour) {
                Object.Destroy(behaviour.gameObject);
            }
            else {
                Object.Destroy(t);
            }
        }
        list.Clear();
    }

    public static void ClearUnityObjectDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dict) where TValue : Object{
        foreach (var pair in dict) {
            Object.Destroy(pair.Value);
        }
        dict.Clear();
    }

}
}