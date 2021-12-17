using System.Collections;
using System.Collections.Generic;

namespace NilanToolkit.Collections {

public class Counter<T> : IEnumerable<KeyValuePair<T,int>> {
    
    private readonly Dictionary<T, int> dic = new Dictionary<T, int>();
    
    public Counter() { }

    public Counter(IEnumerable<T> initialList) {
        foreach (var i in initialList) {
            AddOne(i);
        }
    }

    public void Set(T key, int value) {
        dic[key] = value;
    }

    public void Add(T key, int value) {
        if (dic.ContainsKey(key)) dic[key] += 1;
        else dic[key] = value;
    }

    public void AddOne(T key) {
        Add(key, 1);
    }

    public void Sub(T key, int value) {
        Add(key, -value);
    }

    public void SubOne(T key) {
        Add(key, -1);
    }

    public void Remove(T key) {
        dic.Remove(key);
    }

    public int Get(T key) {
        dic.TryGetValue(key, out var ret);
        return ret;
    }

    public bool TryGetValue(T key, out int value) {
        return dic.TryGetValue(key, out value);
    }

    public bool ContainsKey(T key) {
        return dic.ContainsKey(key);
    }

    public void Clear() {
        dic.Clear();
    }

    public int this[T key] {
        get => Get(key);
        set => Set(key,value);
    }

    public IEnumerator<KeyValuePair<T, int>> GetEnumerator() {
        return dic.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
}