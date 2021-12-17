using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace NilanToolkit.UnityEventHandler {
    public static class UnityEventUtil {
        private static Dictionary<UnityEventType, Type> _handlerTypes = new Dictionary<UnityEventType, Type>() {
            {UnityEventType.ColliderDrag, typeof(ColliderDrag)},
            {UnityEventType.ColliderMouseDown, typeof(ColliderMouseDown)},
            {UnityEventType.ColliderMouseEnter, typeof(ColliderMouseEnter)},
            {UnityEventType.ColliderMouseExit, typeof(ColliderMouseExit)},
            {UnityEventType.ColliderMouseOver, typeof(ColliderMouseOver)},
            {UnityEventType.ColliderMouseUp, typeof(ColliderMouseUp)},
            {UnityEventType.ColliderMouseUpAsButton, typeof(ColliderMouseUpAsButton)},
            {UnityEventType.RendererBecameInvisible, typeof(RendererBecameInvisible)},
            {UnityEventType.RendererBecameVisible, typeof(RendererBecameVisible)}
        };

        public static IUnityEventHandler AddOrGetHandler(this GameObject source, UnityEventType eventType) {
            IUnityEventHandler handler;
            if (_handlerTypes.TryGetValue(eventType, out var T)) {
                var component = source.GetComponent(T);
                if (component == null) {
                    handler = (IUnityEventHandler) source.AddComponent(T);
                }
                else {
                    handler = (IUnityEventHandler) component;
                }

                return handler;
            }
            else {
                throw new Exception("Invalid UnityEventType:" + eventType);
            }
        }

        public static IUnityEventHandler GetHandler(this GameObject source, UnityEventType eventType) {
            IUnityEventHandler handler;
            if (_handlerTypes.TryGetValue(eventType, out var T)) {
                var component = source.GetComponent(T);
                if (component == null) {
                    return null;
                }

                handler = (IUnityEventHandler) component;

                return handler;
            }
            else {
                throw new Exception("Invalid UnityEventType:" + eventType);
            }
        }

        public static void RemoveEventHandler(this GameObject source, UnityEventType eventType) {
            if (_handlerTypes.TryGetValue(eventType, out var T)) {
                var component = source.GetComponent(T);
                if (component == null) {
                    return;
                }

                Object.Destroy(component);
            }
            else {
                throw new Exception("Invalid UnityEventType:" + eventType);
            }
        }

        public static void RemoveAllEventHandlers(this GameObject source) {
            foreach (var t in _handlerTypes.Keys) {
                RemoveEventHandler(source, t);
            }
        }

        public static void AddListener(this GameObject source, UnityEventType eventType, UnityAction listener) {
            var handler = AddOrGetHandler(source, eventType);
            handler.Event.AddListener(listener);
        }

        public static void RemoveListener(this GameObject source, UnityEventType eventType, UnityAction listener) {
            var handler = GetHandler(source, eventType);
            handler?.Event.RemoveListener(listener);
        }

        public static void RemoveAllListeners(this GameObject source, UnityEventType eventType) {
            var handler = GetHandler(source, eventType);
            handler?.Event.RemoveAllListeners();
        }
        
    }
}