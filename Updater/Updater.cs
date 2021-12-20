using System;
using System.Collections.Generic;
using UnityEngine;

namespace NilanToolkit.Updater {
    public class Updater : MonoBehaviour {
        
        private enum OperationType {
            Add,
            Remove
        }

        private struct Operation<T> {
            public OperationType type;
            public T target;
        }
        
        private static Updater _instance;
        private List<IUpdate> _updates = new List<IUpdate>();
        private List<ILateUpdate> _lateUpdates = new List<ILateUpdate>();
        private List<IFixedUpdate> _fixedUpdates = new List<IFixedUpdate>();

        private static Queue<Operation<IUpdate>> _updateOperation = new Queue<Operation<IUpdate>>();
        private static Queue<Operation<ILateUpdate>> _lateUpdateOperation = new Queue<Operation<ILateUpdate>>();
        private static Queue<Operation<IFixedUpdate>> _fixedUpdateOperation = new Queue<Operation<IFixedUpdate>>();

        public static void Register(object target) {
            if (target == null) return;
            EnsureInstance();
            switch (target) {
                case IUpdate update:
                    _updateOperation.Enqueue(new Operation<IUpdate>() {
                        type = OperationType.Add,
                        target = update
                    });
                    break;
                case ILateUpdate lateUpdate:
                    _lateUpdateOperation.Enqueue(new Operation<ILateUpdate>() {
                        type = OperationType.Add,
                        target = lateUpdate
                    });
                    break;
                case IFixedUpdate fixedUpdate:
                    _fixedUpdateOperation.Enqueue(new Operation<IFixedUpdate>() {
                        type = OperationType.Add,
                        target = fixedUpdate
                    });
                    break;
            }
        }

        public static void UnRegister(object target) {
            if (target == null) return;
            if (!_instance) return;
            
            switch (target) {
                case IUpdate update:
                    _updateOperation.Enqueue(new Operation<IUpdate>() {
                        type = OperationType.Remove,
                        target = update
                    });
                    break;
                case ILateUpdate lateUpdate:
                    _lateUpdateOperation.Enqueue(new Operation<ILateUpdate>() {
                        type = OperationType.Remove,
                        target = lateUpdate
                    });
                    break;
                case IFixedUpdate fixedUpdate:
                    _fixedUpdateOperation.Enqueue(new Operation<IFixedUpdate>() {
                        type = OperationType.Remove,
                        target = fixedUpdate
                    });
                    break;
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
            while (_updateOperation.Count > 0) {
                var operation = _updateOperation.Dequeue();
                switch (operation.type) {
                    case OperationType.Add:
                        _updates.Add(operation.target);
                        break;
                    case OperationType.Remove:
                        _updates.Remove(operation.target);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            foreach (var update in _updates) {
                try {
                    update.OnUpdate();
                }
                catch (Exception e) {
                    Debug.LogError(e);
                }
            }
        }

        private void LateUpdate() {
            while (_lateUpdateOperation.Count > 0) {
                var operation = _lateUpdateOperation.Dequeue();
                switch (operation.type) {
                    case OperationType.Add:
                        _lateUpdates.Add(operation.target);
                        break;
                    case OperationType.Remove:
                        _lateUpdates.Remove(operation.target);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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
            while (_fixedUpdateOperation.Count > 0) {
                var operation = _fixedUpdateOperation.Dequeue();
                switch (operation.type) {
                    case OperationType.Add:
                        _fixedUpdates.Add(operation.target);
                        break;
                    case OperationType.Remove:
                        _fixedUpdates.Remove(operation.target);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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