using System;
using System.Collections.Generic;
using UnityEngine;

namespace NilanToolkit.Pool {


    public class ObjectPool : IObjectPool {

        public Func<object> objectLoader;

        protected readonly Stack<object> stack = new Stack<object>();

        public void Collect(object item) {
            if (item is IPoolableObject poolableObject) {
                poolableObject.OnReuse();
            }
            stack.Push(item);

        }

        public object GetItem() {
            object obj = null;
            if (stack.Count > 0) {
                obj = stack.Pop();
            }

            if (objectLoader != null) {
                obj = objectLoader?.Invoke();
            }

            if (obj is IPoolableObject poolableObject) {
                poolableObject.OnReuse();
            }
            return obj;
        }

        public void Dispose() {
            stack.Clear();
        }
    }

    public class ObjectPool<T> : ObjectPool, IObjectPool<T> where T : class {

        public Func<T> objectLoader;

        protected readonly Stack<T> stack = new Stack<T>();

        public virtual void Collect(T item) {
            if (item is IPoolableObject poolableObject) {
                poolableObject.OnReuse();
            }
            stack.Push(item);
        }

        public virtual T GetItem() {
            T obj = null;
            if (stack.Count > 0) {
                obj = stack.Pop();
            }

            if (objectLoader != null) {
                obj = objectLoader?.Invoke();
            }

            if (obj is IPoolableObject poolableObject) {
                poolableObject.OnReuse();
            }
            return obj;
        }

        public virtual void Dispose() {
            stack.Clear();
        }

    }

}
