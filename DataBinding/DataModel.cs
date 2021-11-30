using System;
using System.Collections.Generic;
using System.Linq;
using NilanToolkit.CSharpExtensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NilanToolkit.DataBinding {
    public class DataModel : IDisposable {
        public event Action OnFlushed;

        /// <summary>
        /// 自动建立索引，可以让下次查找速度快些，建议打开
        /// 如果表中存在动态列表，索引可能会失效，此时需要重新建立索引
        /// </summary>
        public bool AutoCreateIndexing { get; set; } = true;
        
        public string Name { get; private set; }

        public bool Dirty { get; set; }

        public readonly Dictionary<BindingObject, Action> events = new Dictionary<BindingObject, Action>();

        internal readonly BindingDictionary main;

        internal readonly Dictionary<string, BindingObject> staticIndexing = new Dictionary<string, BindingObject>();

        private readonly List<BindingObject> _modifiedElements = new List<BindingObject>();

        private DataModelBehaviour _behaviour;

        public DataModel(BindingDictionary dataSource,bool autoFlush = false, string name = null) {
            main = dataSource;
            if (string.IsNullOrEmpty(name)) {
                name = Guid.NewGuid().ToString("N");
            }

            Name = name;

            if (autoFlush) {
                MakeAutoFLush();
            }
        }

        public static DataModel FromJson(string json) {
            if (DataBindingUtil.Parse(json) is BindingDictionary dict) {
                return new DataModel(dict);
            }
            else {
                throw new Exception("data model must be a json object");
            }
        }

        public BindingObject GetElement(DataPath path) {
            BindingObject data;
            if (staticIndexing.TryGetValue(path.Path, out data)) {
                return data;
            }

            data = main.DeepSearchData(path);
            if (data == null) {
                Debug.LogError("get value failed,path:" + path);
                return null;
            }

            if (AutoCreateIndexing) {
                staticIndexing[path.Path] = data;
            }

            return data;
        }

        public void AddElement(DataPath path, string key, BindingObject value) {
            var obj = GetElement(path);
            if (obj is BindingDictionary dict) {
                if (dict.ContainsKey(key)) RemoveElement(path, key);
                dict.Add(key, value);
            }
            else {
                throw new Exception("target is not dictionary");
            }
        }

        public void AddElement(DataPath path, BindingObject value) {
            var obj = GetElement(path);
            if (obj is BindingList list) {
                list.Add(value);
            }
            else {
                throw new Exception("target is not list");
            }
        }

        public void InsertElement(DataPath path,int index, BindingObject value) {
            var obj = GetElement(path);
            if (obj is BindingList list) {
                list.Insert(index, value);
                events.Remove(value);
                foreach (var p in staticIndexing.Keys.ToArray()) {
                    if (path.IsParentOf(p)) {
                        staticIndexing.Remove(p);
                    }
                }
            }
            else {
                throw new Exception("target is not list");
            }
        }

        public void RemoveAt(DataPath path, int index) {
            var obj = GetElement(path);
            if (obj is BindingList list) {
                var item = list.GetItem(index);
                if (item != null) {
                    list.RemoveAt(index);
                    events.Remove(item);
                    foreach (var p in staticIndexing.Keys.ToArray()) {
                        if (path.IsParentOf(p)) {
                            staticIndexing.Remove(p);
                        }
                    }
                }
            }
            else {
                throw new Exception("target is not list");
            }
        }

        public void RemoveElement(DataPath path, string elementKey) {
            var obj = GetElement(path);
            if (obj is BindingDictionary dict) {
                var item = dict.GetItem(elementKey);
                if (item != null) {
                    dict.Remove(elementKey);
                    events.Remove(item);
                    foreach (var p in staticIndexing.Keys.ToArray()) {
                        if (path.IsParentOf(p)) {
                            staticIndexing.Remove(p);
                        }
                    }
                }
            }
            else {
                throw new Exception("target is not dictionary");
            }
        }

        public void RemoveElement(DataPath path, BindingObject item) {
            var obj = GetElement(path);
            if (obj is BindingList list) {
                list.Remove(item);
                events.Remove(item);
                foreach (var p in staticIndexing.Keys.ToArray()) {
                    if (path.IsParentOf(p)) {
                        staticIndexing.Remove(p);
                    }
                }
            }
            else {
                throw new Exception("target is not list");
            }
        }

        public void ClearElements(DataPath path) {
            var obj = GetElement(path);
            if (obj is IBindingList list) {
                list.Clear();
            }
            else if (obj is IBindingDictionary dict) {
                dict.Clear();
            }
        }

        public object GetValue(DataPath path) {
            return GetElement(path)?.value;
        }

        public T GetValue<T>(DataPath path) {
            return (T) GetValue(path);
        }

        public bool SetValue<T>(DataPath path, T value, bool isSetParentDirty = false) {
            if (isSetParentDirty) {
                main.DeepSetValue(value, path, true);
            }
            else {
                var data = GetElement(path);
                data.WriteValue(value);
            }

            SetDirty();
            return true;
        }

        public bool SetValue(DataPath path, object value, bool isSetParentDirty = false) {
            if (isSetParentDirty) {
                main.DeepSetValue(value, path, true);
            }
            else {
                var data = GetElement(path);
                data.WriteValue(value);
            }

            SetDirty();
            return true;
        }

        public void SetDirty() {
            Dirty = true;
        }

        public void AddDataListener(BindingObject obj, Action evt) {
            if (events.TryGetValue(obj, out var @event)) {
                @event += evt;
                events[obj] = @event;
            }
            else {
                events[obj] = evt;
            }
        }

        public void AddDataListener(DataPath path, Action evt) {
            AddDataListener(GetElement(path), evt);
        }

        public void RemoveDataListener(BindingObject obj, Action evt) {
            if (events.TryGetValue(obj, out var @event)) {
                @event -= evt;
                events[obj] = @event;
            }
        }

        public void RemoveDataListener(DataPath path, Action evt) {
            RemoveDataListener(GetElement(path), evt);
        }

        public void ClearDataListener(BindingObject obj) {
            events.Remove(obj);
        }

        public void ClearDataListener(DataPath path) {
            ClearDataListener(GetElement(path));
        }

        public void ClearAllListener() {
            events.Clear();
        }

        /// <summary>
        /// 清洗所有脏数据，下发数据变更的通知
        /// </summary>
        /// <param name="force">无视脏标记状态强制刷新</param>
        public void Flush(bool force = false) {
            if (!Dirty && !force) return;
            Dirty = false;

            _modifiedElements.Clear();

            void Operation(BindingObject data) {
                if (!data.Dirty) return;
                data.Dirty = false;
                _modifiedElements.Add(data);
            }

            main.IteratesAllChildElements(Operation, false);

            foreach (var d in _modifiedElements) {
                try {
                    TryTriggerDataEvent(d);
                }
                catch (Exception e) {
                    Debug.LogError(e.ToString());
                }
            }

            OnFlushed?.Invoke();
        }

        public void UpdatePathMapping() {
            var keys = staticIndexing.Keys.ToArray();
            foreach (var key in keys) {
                var data = GetElement(key);
                if (data == null) {
                    staticIndexing.Remove(key);
                }
                else {
                    staticIndexing[key] = data;
                }
            }
        }

        public void MakeAutoFLush(bool enable = true) {
            if (enable) {
                if (_behaviour) return;
                var go = new GameObject("DataModelBehaviour-" + Name);
                _behaviour = go.AddComponent<DataModelBehaviour>();
                _behaviour.OnLateUpdate += () => { Flush(); };
            }
            else {
                if (!_behaviour) return;
                Object.Destroy(_behaviour.gameObject);
                _behaviour = null;
            }
        }

        private void TryTriggerDataEvent(BindingObject obj) {
            if (events.TryGetValue(obj, out var evt)) {
                evt?.Invoke();
            }
        }

        public void Dispose() {
            if (_behaviour) {
                Object.Destroy(_behaviour);
            }
        }
    }
}