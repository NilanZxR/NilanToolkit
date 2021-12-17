using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NilanToolkit.Pool {

    /// <summary>
    /// a pool can put-in/take-out GameObjects 
    /// </summary>
    public class GameObjectPool : ObjectPool<GameObject> {

        private bool _useIndependentContainer;

        /// <summary>
        /// put item under independent root. otherwise keep it under original root
        /// if it is any item in pool, set to true will put them to Container immediately, or set to false will put them to scene-root
        /// </summary>
        public bool UseIndependentContainer {
            get {
                return _useIndependentContainer;
            }
            set {
                if (value == _useIndependentContainer) return;
                if (value) {
                    if (!Container) {
                        Container = new GameObject($"ObjectPool--{PoolName}").transform;
                    }
                    foreach (var go in stack) {
                        go.transform.SetParent(Container);
                    }
                }
                else {
                    foreach (var gameObject in stack) {
                        gameObject.transform.SetParent(null);
                    }
                    if (Container) {
                        Object.Destroy(Container.gameObject);
                    }
                }
            }
        }


        /// <summary>
        /// object container, set UseIndependentContainer to enable container
        /// </summary>
        public Transform Container { get; private set; }

        public GameObjectPool() { }

        public GameObjectPool(Loader<GameObject> loader) : base(loader) { }

        public override GameObject GetItem() {
            var item = base.GetItem();
            item.SetActive(true);
            return item;
        }

        public override void Collect(GameObject item) {
            item.SetActive(false);
            if (_useIndependentContainer) item.transform.SetParent(Container);
            base.Collect(item);
        }

        public void DontDestroyOnLoad() {
            if (!_useIndependentContainer) {
                throw new Exception("Use independent container before Set DontDestroyOnLoad!");
            }
            Object.DontDestroyOnLoad(Container);
        }

        public override void Dispose() {
            while (stack.Count > 0) {
                var gameObject = stack.Pop();
                Object.Destroy(gameObject);
            }

            if (Container) {
                Object.Destroy(Container.gameObject);
            }
            base.Dispose();
        }

        public override void DisposeAndKeepItemsAlive() {
            UseIndependentContainer = false;
            base.DisposeAndKeepItemsAlive();
        }

        protected override bool GetInterface(GameObject item, out IPoolEventHandler interfaceInst) {
            interfaceInst = item.GetComponent<IPoolEventHandler>();
            return interfaceInst != null;
        }
    }
}
