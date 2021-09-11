using UnityEngine;

namespace NilanToolkit.Pool {

    public class MonoBehaviourPool : ObjectPool<MonoBehaviour> {

        public override void Collect(MonoBehaviour item) {
            item.gameObject.SetActive(false);
            base.Collect(item);
        }

        public override MonoBehaviour GetItem() {
            var item = base.GetItem();
            item.gameObject.SetActive(true);
            return item;
        }

        public override void Dispose() {
            while (stack.Count > 0) {
                var obj = stack.Pop();
                Object.Destroy(obj.gameObject);
            }
        }

    }

    public class MonoBehaviourPool<T> : ObjectPool<T> where T : MonoBehaviour {
        
        public override void Collect(T item) {
            item.gameObject.SetActive(false);
            base.Collect(item);
        }

        public override T GetItem() {
            var item = base.GetItem();
            item.gameObject.SetActive(true);
            return item;
        }

        public override void Dispose() {
            while (stack.Count > 0) {
                var obj = stack.Pop();
                Object.Destroy(obj.gameObject);
            }
        }

    }
}
