using System;
using UnityEngine;
using UnityEngine.Events;

namespace NilanToolkit.UnityEventHandler {
    [RequireComponent(typeof(Renderer))]
    public class RendererBecameVisible : MonoBehaviour, IUnityEventHandler {
        public UnityEvent onBecameVisible;
        private void OnBecameVisible() {
            onBecameVisible.Invoke();
        }

        public UnityEvent Event => onBecameVisible;
    }
}