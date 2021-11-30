using System;
using System.Collections.Generic;
using UnityEngine;

namespace NilanToolkit.DirtyMark {
    
    /// <summary>
    /// 一个简单的脏标记系统
    /// 使用方式: 1.实现IDirtyMarkable接口，2.DirtyMark.SetDirty()
    /// </summary>
    public class DirtyMark : MonoBehaviour{

        private static DirtyMark _instance;
        private readonly Stack<IDirtyMarkable> _stack = new Stack<IDirtyMarkable>();

        private readonly Dictionary<string, List<IDirtyMarkable>> _channels =
            new Dictionary<string, List<IDirtyMarkable>>();


        private static DirtyMark Instance {
            get {
                if (!_instance) {
                    var go = new GameObject("DirtyMarkBehaviour");
                    _instance = go.AddComponent<DirtyMark>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        public static bool Active {
            get => _instance && _instance.gameObject.activeSelf;
            set => Instance.gameObject.SetActive(value);
        }
        
        public static bool AutoFlush { get; set; }

        public static void SetDirty(IDirtyMarkable target) {
            Instance._stack.Push(target);
        }

        public static void SetDirty(GameObject gameObject) {
            var arr = gameObject.GetComponents<IDirtyMarkable>();
            for (var i = 0; i < arr.Length; i++) {
                SetDirty(arr[i]);
            }
        }
        
        public static void SetDirty(string channelName) {
            if (Instance._channels.TryGetValue(channelName, out var channel)) {
                for (var i = 0; i < channel.Count; i++) {
                    SetDirty(channel[i]);
                }
            }
        }
 
        public static void Subscribe(string channelName, IDirtyMarkable target) {
            if (!Instance._channels.TryGetValue(channelName, out var channel)) {
                channel = new List<IDirtyMarkable>();
                Instance._channels[channelName] = channel;
            }

            channel.Add(target);
        }

        public static void SubscribeRange(string channelName, IEnumerable<IDirtyMarkable> targets) {
            if (!Instance._channels.TryGetValue(channelName, out var channel)) {
                channel = new List<IDirtyMarkable>();
                Instance._channels[channelName] = channel;
            }

            foreach (var target in targets) {
                channel.Add(target);   
            }
        }

        public static void Unsubscribe(string channelName, IDirtyMarkable target) {
            if (Instance._channels.TryGetValue(channelName, out var channel)) {
                channel.Remove(target);
            }
        }

        public static void DisposeChannel(string channelName) {
            if (Instance._channels.TryGetValue(channelName, out var channel)) {
                channel.Clear();
                Instance._channels.Remove(channelName);
            }
        }

        /// <summary>
        /// 清洗所有脏标记
        /// </summary>
        public static void Flush() {
            var stack = Instance._stack;
            while (stack.Count > 0) {
                var item = stack.Pop();
                try {
                    item.OnDirtyStateRefresh();
                }
                catch (Exception e) {
                    Debug.LogError(e);
                }
            }
        }
        
        public static void Dispose() {
            if (_instance) {
                _instance._stack.Clear();
                _instance._channels.Clear();
                Destroy(_instance.gameObject);
            }
        }

        private void LateUpdate() {
            if (AutoFlush) Flush();
        }
        
    }
    
}