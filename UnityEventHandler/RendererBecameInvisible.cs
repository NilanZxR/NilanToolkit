using System;
using UnityEngine;
using UnityEngine.Events;

namespace NilanToolkit.UnityEventHandler {
    [RequireComponent(typeof(Renderer))]
    public class RendererBecameInvisible : MonoBehaviour, IUnityEventHandler {
        public UnityEvent onBecameInvisible;
        
        private void OnBecameInvisible() {
            onBecameInvisible.Invoke();
        }

        public UnityEvent Event => onBecameInvisible;
    }
}