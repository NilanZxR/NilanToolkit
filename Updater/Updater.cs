using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NilanToolkit.Updater {
    public class Updater : MonoBehaviour {
        
        private static Updater _instance;
        private List<IUpdate> _updates = new List<IUpdate>();
        private List<ILateUpdate> _lateUpdates = new List<ILateUpdate>();
        private List<IFixedUpdate> _fixedUpdates = new List<IFixedUpdate>();

        public static void Register(object target) {
            if (target == null) return;
            EnsureInstance();
            if (target is IUpdate update) {
                _instance._updates.Add(update);
            }

            if (target is ILateUpdate lateUpdate) {
                _instance._lateUpdates.Add(lateUpdate);
            }

            if (target is IFixedUpdate fixedUpdate) {
                _instance._fixedUpdates.Add(fixedUpdate);
            }
        }

        public static void UnRegister(object target) {
            if (target == null) return;
            if (!_instance) return;
            
            if (target is IUpdate update) {
                _instance._updates.Remove(update);
            }

            if (target is ILateUpdate lateUpdate) {
                _instance._lateUpdates.Remove(lateUpdate);
            }

            if (target is IFixedUpdate fixedUpdate) {
                _instance._fixedUpdates.Remove(fixedUpdate);
            }
        }

        public static void Clear() {
            if (!_instance) return;
            
            _instance._updates.Clear();
            _instance._lateUpdates.Clear();
            _instance._fixedUpdates.Clear();
        }

        private static void EnsureInstance() {
            if (_instance) return;

            var go = new GameObject("Updater");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<Updater>();
        }

        private void Update() {
            foreach (var update in _updates) {
                try {
                    update.OnUpdate();
                }
                catch (Exception e) {
                    Debug.Log(e);
                }
            }
        }

        private void LateUpdate() {
            foreach (var lateUpdate in _lateUpdates) {
                try {
                    lateUpdate.OnLateUpdate();
                }
                catch (Exception e) {
                    Debug.Log(e);
                }
            }
        }

        private void FixedUpdate() {
            foreach (var fixedUpdate in _fixedUpdates) {
                try {
                    fixedUpdate.OnFixedUpdate();
                }
                catch (Exception e) {
                    Debug.Log(e);
                }
            }
        }
    }
}