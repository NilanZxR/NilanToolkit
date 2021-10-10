using System;
using System.Collections;
using System.Collections.Generic;
using NilanToolkit.CSharpExtensions;

namespace NilanToolkit.DataModel {
    public class ObservedArray : ObservedObject, IObservedCollection {
        
        private readonly List<ObservedObject> _arr = new List<ObservedObject>();
    
        public void Add(ObservedObject val) {
            _arr.Add(val);
        }

        public IEnumerator<ObservedObject> GetEnumerator() {
            return _arr.GetEnumerator();
        }

        public int FindIndex(Predicate<ObservedObject> predicate) {
            return _arr.FindIndex(predicate);
        }

        public ObservedObject GetItem(string key) {
            if (int.TryParse(key,out var index)) {
                if (_arr.IsInRange(index)) {
                    return _arr[index];
                }
                else {
                    throw new Exception("index out of range");
                }
            }
            else {
                throw new Exception("invalid index");
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    
        internal override ObservedObject Search(ref Stack<string> stack) {
            if (stack.Count <= 0) return this;
            var key = stack.Pop();
            if (int.TryParse(key,out var index)) {
                if (_arr.IsInRange(index)) {
                    return _arr[index].Search(ref stack);
                }
                else {
                    throw new Exception("index out of range");
                }
            }
            else {
                throw new Exception("invalid index");
            }
        }

    }
}
