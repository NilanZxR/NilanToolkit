using UnityEngine;
using UnityEngine.Events;

namespace NilanToolkit.UnityEventHandler {
    [RequireComponent(typeof(Collider))]
    public class ColliderMouseDown : MonoBehaviour , IUnityEventHandler{
        public UnityEvent onMouseDown;

        private void OnMouseDown() {
            onMouseDown.Invoke();
        }

        public UnityEvent Event => onMouseDown;
    }
}