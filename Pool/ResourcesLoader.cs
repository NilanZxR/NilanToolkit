using UnityEngine;

namespace NilanToolkit.Pool {
    public class ResourcesLoader : MonoBehaviour, IGameObjectLoader {
        
        public string path;
        
        public GameObject Create() {
            var prefab = Resources.Load<GameObject>(path);
            var inst = Instantiate(prefab);
            return inst;
        }
        
    }
}