using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NilanToolkit.Pool {

    public class GameObjectPool : ObjectPool<GameObject> {

        private bool _useIndependentContainer;

        public bool UseIndependentContainer {
            get {
                return _useIndependentContainer;
            }
            set {
                if (value) {
                    if (!container) {
                        container = new GameObject($"ObjectPool--{PoolName}").transform;
                    }
                    foreach (var go in stack) {
                        go.transform.SetParent(container);
                    }
                }
                else {
                    foreach (var gameObject in stack) {
                        gameObject.transform.SetParent(null);
                    }
                    if (container) {
                        Object.Destroy(container.gameObject);
                    }
                }
            }
        }


        private Transform container;

        public GameObjectPool() { }

        public GameObjectPool(Loader<GameObject> loader) : base(loader) { }

        public override GameObject GetItem() {
            var item = base.GetItem();
            item.SetActive(true);
            return item;
        }

        public override void Collect(GameObject item) {
            item.SetActive(false);
            if (_useIndependentContainer) item.transform.SetParent(container);
            base.Collect(item);
        }

        protected override void DisposeAllObject() {
            while (stack.Count > 0) {
                var gameObject = stack.Pop();
                Object.Destroy(gameObject);
            }
        }

        protected override bool GetInterface(GameObject item, out IPoolableObject interfaceInst) {
            interfaceInst = item.GetComponent<IPoolableObject>();
            return interfaceInst != null;
        }
    }
}
