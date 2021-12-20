using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NilanToolkit.Collections {
    /// <summary>
    /// 类似List，但删除时会将元素交换到尾部，不会造成额外开销，但无法保障元素顺序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class ResizeableArray<T> :
        IList,
        IList<T> 
    {
        private T[] _arr;

        public int Capacity => _arr.Length;
        public int Count { get; private set; }
        
        public bool IsSynchronized => _arr.IsSynchronized;
        
        public object SyncRoot => _arr.SyncRoot;
        
        public bool IsReadOnly => false;
        
        public bool IsFixedSize => false;

        /// <summary>
        /// index of last item
        /// </summary>
        private int Tail => Count == 0 ? 0 : Count - 1;

        object IList.this[int index] {
            get => this[index];
            set => this[index] = (T) value;
        }

        public T this[int index] {
            get {
                if (index < 0 || index >= Count) {
                    throw new IndexOutOfRangeException();
                }
                return _arr[index];
            }
            set => Add(value); // 列表元素位置不固定，因此在指定位置插入值没有意义
        }

        public ResizeableArray() : this(8) { }

        public ResizeableArray(int capacity) {
            _arr = new T[capacity];
        }

        public void Add(T item) {
            if (Count >= _arr.Length) {
                Resize(Capacity * 2);
            }
            _arr[Count] = item;
            Count++;
        }

        public int Add(object value) {
            Add((T) value);
            return Tail;
        }

        public void Clear() {
            for (var i = 0; i < Count; i++) {
                _arr[i] = default;
            }
            Count = 0;
        }

        public bool Contains(object value) {
            return value != default && Contains((T) value);
        }

        public int IndexOf(object value) {
            if (value == default) return -1;
            return IndexOf((T) value);
        }

        public void Insert(int index, object value) {
            Add(value);
        }

        public void Remove(object value) {
            if (value == default) return;
            Remove((T) value);
        }

        public bool Contains(T item) {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex) {
            for (var i = 0; i < Count; i++) {
                array.SetValue(_arr[i], arrayIndex++);
            }
        }

        public void CopyTo(Array array, int index) {
            for (var i = 0; i < Count; i++) {
                array.SetValue(_arr[i], index++);
            }
        }

        public bool Remove(T item) {
            var index = IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        public void Insert(int index, T item) {
            Add(item); // 列表元素位置不固定，因此在指定位置插入值没有意义
        }
        public void RemoveAt(int index) {
            if (index > Tail || Count == 0) return;
            if (index < Tail) {
                Wrap(index, Tail);
            }
            _arr[Tail] = default;
            Count--;
        }

        public int IndexOf(T item) {
            for (var i = 0; i < Count; i++) {
                if (_arr[i].Equals(item)) return i;
            }
            return -1;
        }

        public void Resize(int newSize) {
            if (Count > newSize) {
                Count = newSize;
            }
            Array.Resize(ref _arr, newSize);
        }

        private void Wrap(int a, int b) {
            var tmp = _arr[a];
            _arr[a] = _arr[b];
            _arr[b] = tmp;
        }

        public IEnumerator<T> GetEnumerator() {
            for (var i = 0; i < Count; i++) {
                yield return _arr[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
