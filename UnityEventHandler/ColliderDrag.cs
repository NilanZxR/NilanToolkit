using UnityEngine;
using UnityEngine.Events;

namespace NilanToolkit.UnityEventHandler {
    [RequireComponent(typeof(Collider))]
    public class ColliderDrag : MonoBehaviour, IUnityEventHandler{
        public UnityEvent onDrag;

        private void OnMouseDrag() {
            onDrag.Invoke();
        }

        public UnityEvent Event => onDrag;
    }
}