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
        private static readonly Stack<IDirtyMarkable> Stack = new Stack<IDirtyMarkable>();

        private static readonly Dictionary<string, List<IDirtyMarkable>> Channels =
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

        public static bool AutoFlush {
            get => _instance && _instance.gameObject.activeSelf;
            set => Instance.gameObject.SetActive(value);
        }

        public static void SetDirty(IDirtyMarkable target) {
            Stack.Push(target);
        }

        public static void SetDirty(GameObject gameObject) {
            var arr = gameObject.GetComponents<IDirtyMarkable>();
            for (var i = 0; i < arr.Length; i++) {
                SetDirty(arr[i]);
            }
        }
        
        public static void SetDirty(string channelName) {
            if (Channels.TryGetValue(channelName, out var channel)) {
                for (var i = 0; i < channel.Count; i++) {
                    SetDirty(channel[i]);
                }
            }
        }
 
        public static void Subscribe(string channelName, IDirtyMarkable target) {
            if (!Channels.TryGetValue(channelName, out var channel)) {
                channel = new List<IDirtyMarkable>();
                Channels[channelName] = channel;
            }

            channel.Add(target);
        }

        public static void SubscribeRange(string channelName, IEnumerable<IDirtyMarkable> targets) {
            if (!Channels.TryGetValue(channelName, out var channel)) {
                channel = new List<IDirtyMarkable>();
                Channels[channelName] = channel;
            }

            foreach (var target in targets) {
                channel.Add(target);   
            }
        }

        public static void Unsubscribe(string channelName, IDirtyMarkable target) {
            if (Channels.TryGetValue(channelName, out var channel)) {
                channel.Remove(target);
            }
        }

        public static void DisposeChannel(string channelName) {
            if (Channels.TryGetValue(channelName, out var channel)) {
                channel.Clear();
                Channels.Remove(channelName);
            }
        }

        /// <summary>
        /// 清洗所有脏标记
        /// </summary>
        public static void Flush() {
            while (Stack.Count > 0) {
                var item = Stack.Pop();
                try {
                    item.OnDirtyStateRefresh();
                }
                catch (Exception e) {
                    Debug.LogError(e);
                }
            }
        }
        
        public static void Dispose() {
            Stack.Clear();
            Channels.Clear();
            if (_instance) {
                Destroy(_instance.gameObject);
            }
        }

        private void LateUpdate() {
            Flush();
        }
        
    }
    
}