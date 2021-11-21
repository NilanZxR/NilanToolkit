using System;

namespace NilanToolkit.Pool {

    public interface IObjectPool<T> : IDisposable {

        void Collect(T item);

        T GetItem();

    }
    
}
