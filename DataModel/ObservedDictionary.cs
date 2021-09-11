using System;
using System.Collections;
using System.Collections.Generic;

namespace NilanToolkit.DataModel {
    public class ObservedDictionary : ObservedObject, IObservedCollection {
          
        public Dictionary<string, ObservedObject>.Enumerator Iter => _dictionary.GetEnumerator();

        public Dictionary<string, ObservedObject>.KeyCollection Keys => _dictionary.Keys;

        public Dictionary<string, ObservedObject>.ValueCollection Values => _dictionary.Values;

        private readonly Dictionary<string, ObservedObject> _dictionary = new Dictionary<string, ObservedObject>();

        public void Add(string key, ObservedObject data) {
            _dictionary.Add(key, data);
        }

        public IEnumerator<ObservedObject> GetEnumerator() {
            return _dictionary.Values.GetEnumerator();
        }

        public ObservedObject GetItem(string key) {
            if (_dictionary.TryGetValue(key,out var value)) {
                return value;
            }
            else {
                return null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        internal override ObservedObject Search(ref Stack<string> stack) {
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
