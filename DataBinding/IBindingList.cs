using System;

namespace NilanToolkit.DataBinding {
    public interface IBindingList : IDataBindingCollection {
        void Add(BindingObject val);
        void Remove(BindingObject val);
        void Insert(int index, BindingObject val);
        void RemoveAt(int index);
        int IndexOf(BindingObject predicate);
        BindingObject GetItem(int index);
        void Clear();
    }
}