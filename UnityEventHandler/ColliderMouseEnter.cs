using UnityEngine;
using UnityEngine.Events;

namespace NilanToolkit.UnityEventHandler {
    [RequireComponent(typeof(Collider))]
    public class ColliderMouseEnter : MonoBehaviour , IUnityEventHandler{
        public UnityEvent onMouseEnter;

        private void OnMouseEnter() {
            onMouseEnter.Invoke();
        }

        public UnityEvent Event => onMouseEnter;
    }
}