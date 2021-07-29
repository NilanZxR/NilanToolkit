using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace NilanToolkit {

/// <summary>
/// random select item by weight
/// </summary>
/// <typeparam name="T"></typeparam>
public class RandomGroup<T> {

    protected readonly List<KeyValuePair<T, int>> items = new List<KeyValuePair<T, int>>();

    public int Total { get; protected set; }

    public RandomGroup(){ }
    
    public RandomGroup(IEnumerable<KeyValuePair<T,int>> initItems) {
        foreach (var item in initItems) {
            items.Add(new KeyValuePair<T, int>(item.Key, item.Value));
            Total += item.Value;
        }
    }

    public void Add(T key, int weight) {
        Add(new KeyValuePair<T, int>(key, weight));
    }

    public void Add(KeyValuePair<T, int> item){
        items.Add(item);
        Total += item.Value;
    }

    public void Remove(T key) {
        int w = 0;
        for (var index = 0; index < items.Count; index++) {
            var item = items[index];
            if (item.Key.Equals(key)) {
                w += item.Value;
                items.RemoveAt(index);
                index--;
            }
        }

        Total -= w;
    }

    public void Clear(){
        items.Clear();
        Total = 0;
    }

    /// <summary>
    /// pick item by weight
    /// </summary>
    /// <returns></returns>
    public T Pick() {
        if (items.Count == 0) {
            throw new Exception("no item in list");
        }
        var w = Random.Range(1, Total + 1);
        foreach (var item in items) {
            w -= item.Value;
            if (w <= 0) return item.Key;
        }
        
        return items[0].Key;
    }

}
}