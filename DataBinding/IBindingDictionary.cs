using System.Collections.Generic;

namespace NilanToolkit.DataBinding {
    public interface IBindingDictionary : IDataBindingCollection {
        Dictionary<string, BindingObject>.KeyCollection Keys { get; }
        Dictionary<string, BindingObject>.ValueCollection Values { get; }
        void Add(string key, BindingObject data);
        void Remove(string key);
        bool ContainsKey(string key);
        bool ContainsValue(BindingObject val);
        bool TryGetValue(string key, out BindingObject val);
        void Clear();
    }
}