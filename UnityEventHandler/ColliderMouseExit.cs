using UnityEngine;
using UnityEngine.Events;

namespace NilanToolkit.UnityEventHandler {
    [RequireComponent(typeof(Collider))]
    public class ColliderMouseExit : MonoBehaviour, IUnityEventHandler {
        public UnityEvent onMouseExit;

        private void OnMouseExit() {
            onMouseExit.Invoke();
        }

        public UnityEvent Event => onMouseExit;
    }
}