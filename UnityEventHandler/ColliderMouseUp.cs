using UnityEngine;
using UnityEngine.Events;

namespace NilanToolkit.UnityEventHandler {
    [RequireComponent(typeof(Collider))]
    public class ColliderMouseUp : MonoBehaviour, IUnityEventHandler {
        public UnityEvent onMouseUp;

        private void OnMouseUp() {
            onMouseUp.Invoke();
        }

        public UnityEvent Event => onMouseUp;
    }
}