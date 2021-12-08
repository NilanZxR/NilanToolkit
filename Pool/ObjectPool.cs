using System;
using System.Collections.Generic;
using UnityEngine;

namespace NilanToolkit.Pool {

    public class ObjectPool<T> :  IObjectPool<T> where T : class {
        
        public Loader<T> objectLoader;

        public bool CreateObjectWhenStackEmpty;
        
        public readonly Stack<T> stack = new Stack<T>();

        public string PoolName { get; set; }
        
        public ObjectPool() {
            CreateObjectWhenStackEmpty = false;
        }

        public ObjectPool(Loader<T> objectLoader) {
            CreateObjectWhenStackEmpty = true;
            this.objectLoader = objectLoader;
        }

        public virtual void Collect(T item) {
            if (GetInterface(item, out var interfaceInst)) {
                interfaceInst.OnCollect();
            }

            stack.Push(item);
        }

        public virtual T GetItem() {
            T obj = null;
            while (stack.Count > 0 && obj == null) {
                obj = stack.Pop();
            }
            if (obj != null) {
                if (GetInterface(obj, out var interfaceInst)) {
                    interfaceInst.OnReuse();
                }

                return obj;
            }
            
            if (CreateObjectWhenStackEmpty) {
                return Create();
            }
            else {
                throw new PoolingException("stack empty!");
            }
        }

        public void Preload(int count) {
            if (objectLoader == null) throw new PoolingException("loader is not register");
            for (var i = 0; i < count; i++) {
                var item = objectLoader();
                Collect(item);
            }
        }

        public virtual void Dispose() {
            DisposeAllObject();
            stack.Clear();
        }

        protected virtual T Create() {
            if (objectLoader == null) {
                throw new PoolingException("object loader is not registered");
            }

            var obj = objectLoader.Invoke();
            if (obj == null) {
                throw new PoolingException("the object you created is null");
            }

            if (GetInterface(obj, out var interfaceInst)) {
                interfaceInst.OnCreate();
            }

            return obj;
        }

        protected virtual void DisposeAllObject() { }

        protected virtual bool GetInterface(T item, out IPoolableObject interfaceInst) {
            interfaceInst = item as IPoolableObject;
            return interfaceInst != null;
        }
        
    }
}