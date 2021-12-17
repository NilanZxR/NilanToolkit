using UnityEngine;
using UnityEngine.Events;

namespace NilanToolkit.UnityEventHandler {
    [RequireComponent(typeof(Collider))]
    public class ColliderMouseUpAsButton : MonoBehaviour, IUnityEventHandler {
        public UnityEvent onMouseUpAsButton;
        
        private void OnMouseUpAsButton() {
            onMouseUpAsButton.Invoke();
        }

        public UnityEvent Event => onMouseUpAsButton;
    }
}