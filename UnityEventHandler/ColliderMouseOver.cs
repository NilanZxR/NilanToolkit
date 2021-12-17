using UnityEngine;
using UnityEngine.Events;

namespace NilanToolkit.UnityEventHandler {
    [RequireComponent(typeof(Collider))]
    public class ColliderMouseOver : MonoBehaviour, IUnityEventHandler {
        public UnityEvent onMouseOver;

        private void OnMouseOver() {
            onMouseOver.Invoke();
        }

        public UnityEvent Event => onMouseOver;
    }
}