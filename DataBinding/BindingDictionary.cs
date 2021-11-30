using System;
using System.Collections;
using System.Collections.Generic;

namespace NilanToolkit.DataBinding {
    public class BindingDictionary : BindingObject, IBindingDictionary {
          
        public Dictionary<string, BindingObject>.Enumerator Iter => _dictionary.GetEnumerator();

        public Dictionary<string, BindingObject>.KeyCollection Keys => _dictionary.Keys;

        public Dictionary<string, BindingObject>.ValueCollection Values => _dictionary.Values;

        private readonly Dictionary<string, BindingObject> _dictionary = new Dictionary<string, BindingObject>();

        public void Add(string key, BindingObject data) {
            _dictionary.Add(key, data);
            SetDirty();
        }

        public void Remove(string key) {
            _dictionary.Remove(key);
            SetDirty();
        }

        public bool ContainsKey(string key) {
            return _dictionary.ContainsKey(key);
        }

        public bool ContainsValue(BindingObject val) {
            return _dictionary.ContainsValue(val);
        }

        public bool TryGetValue(string key, out BindingObject val) {
            return _dictionary.TryGetValue(key, out val);
        }

        public IEnumerator<BindingObject> GetEnumerator() {
            return _dictionary.Values.GetEnumerator();
        }

        public BindingObject GetItem(string key) {
            if (_dictionary.TryGetValue(key,out var value)) {
                return value;
            }
            else {
                return null;
            }
        }

        public void Clear() {
            _dictionary.Clear();
            SetDirty();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        internal override BindingObject Search(ref Stack<string> stack) {
            if (stack.Count <= 0) return this;
            var key = stack.Pop();
            if (_dictionary.TryGetValue(key,out var value)) {
                return value.Search(ref stack);
            }
            else {
                throw new Exception("given path not found:" + key);
            }
        }

    }
}
