using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NilanToolkit.Pool {

    public class GameObjectPool : ObjectPool<GameObject> {

        public override void Collect(GameObject item) {
            item.SetActive(false);
            base.Collect(item);
        }

        public override GameObject GetItem() {
            var obj = base.GetItem();
            obj.SetActive(true);
            return obj;
        }

        public override void Dispose() {
            while (stack.Count > 0) {
                var obj = stack.Pop();
                Object.Destroy(obj);
            }
        }

    }
}
