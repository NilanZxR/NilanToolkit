using System;

namespace NilanToolkit.Pool {

    public interface IObjectPool : IDisposable {

        void Collect(object item);

        object GetItem();

    }

    public interface IObjectPool<T> : IDisposable {

        void Collect(T item);

        T GetItem();

    }
}
