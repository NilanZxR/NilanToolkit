using System;
using System.Collections;
using System.Collections.Generic;
using NilanToolkit.CSharpExtensions;

namespace NilanToolkit.DataBinding {
    public class BindingList : BindingObject, IBindingList {
        
        private readonly List<BindingObject> _list = new List<BindingObject>();
    
        public void Add(BindingObject val) {
            _list.Add(val);
            SetDirty();
        }

        public void Remove(BindingObject val) {
            _list.Remove(val);
            SetDirty();
        }

        public void Insert(int index, BindingObject val) {
            _list.Insert(index, val);
            SetDirty();
        }

        public void RemoveAt(int index) {
            _list.RemoveAt(index);
            SetDirty();
        }

        public IEnumerator<BindingObject> GetEnumerator() {
            return _list.GetEnumerator();
        }

        public int IndexOf(BindingObject obj) {
            return _list.IndexOf(obj);
        }

        public BindingObject GetItem(int index) {
            if (_list.IsInRange(index)) {
                return _list[index];
            }
            else {
                throw new Exception("index out of range");
            }
        }

        public BindingObject GetItem(string key) {
            if (int.TryParse(key,out var index)) {
                return GetItem(index);
            }
            else {
                throw new Exception("invalid index");
            }
        }

        public void Clear() {
            _list.Clear();
            SetDirty();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    
        internal override BindingObject Search(ref Stack<string> stack) {
            if (stack.Count <= 0) return this;
            var key = stack.Pop();
            if (int.TryParse(key,out var index)) {
                if (_list.IsInRange(index)) {
                    return _list[index].Search(ref stack);
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
