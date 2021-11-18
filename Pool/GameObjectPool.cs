using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NilanToolkit.Pool {

    public class GameObjectPool : ObjectPool<GameObject> {

        public GameObjectPool() { }

        public GameObjectPool(Loader<GameObject> loader) : base(loader) { }

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
