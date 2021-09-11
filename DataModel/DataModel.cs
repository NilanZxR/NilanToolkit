using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NilanToolkit.DataModel {
    public class DataModel {

        public event Action onFlushed;

        public IndexingMode IndexingMode { get; set; }

        public bool Dirty { get; set; }

        public readonly Dictionary<ObservedObject, DataEvent> Events = new Dictionary<ObservedObject, DataEvent>();

        internal readonly ObservedDictionary Main;

        internal readonly Dictionary<string, ObservedObject> PathMapper = new Dictionary<string, ObservedObject>();

        private readonly List<ObservedObject> _eventTriggers = new List<ObservedObject>();

        public DataModel(ObservedDictionary dataSource)
        {
            Main = dataSource;
        }

        public DataModel(string json)
        {
            if (DataModelUtil.Parse(json) is ObservedDictionary dict) {
                Main = dict;
            }
            else {
                throw new Exception("data model must be a json object");
            }
        }

        public ObservedObject GetData(DataPath path)
        {
            ObservedObject data;
            PathMapper.TryGetValue(path.Path, out data);
            data = Main.DeepSearchData(path);
            if (data == null) {
                Debug.LogError("get value failed,path:" + path);
                return null;
            }

            if (IndexingMode == IndexingMode.Lazy) {
                PathMapper[path.Path] = data;
            }

            return data;
        }

        public ObservedObject GetData(string path) => GetData(DataPath.Create(path));

        public T GetValue<T>(string path) => GetValue<T>(DataPath.Create(path));

        public T GetValue<T>(DataPath path)
        {
            return (T) GetValue(path);
        }

        public object GetValue(string path) => GetValue(DataPath.Create(path));

        public object GetValue(DataPath path)
        {
            return GetData(path)?.value;
        }

        public bool SetValue<T>(string path, T value, bool isSetParentDirty = false) => SetValue(DataPath.Create(path), value, isSetParentDirty);

        public bool SetValue<T>(DataPath path, T value, bool isSetParentDirty = false)
        {
            if (isSetParentDirty) {
                Main.DeepSetValue(value, path, true);
            }
            else {
                var data = GetData(path);
                //todo need check value type
                data.WriteValue(value);
            }
            SetDirty();
            return true;
        }

        public bool SetValue(string path, object value, bool isSetParentDirty = false) =>
            SetValue(DataPath.Create(path), value, isSetParentDirty);

        public bool SetValue(DataPath path, object value, bool isSetParentDirty = false)
        {
            if (isSetParentDirty) {
                Main.DeepSetValue(value, path, true);
            }
            else {
                var data = GetData(path);
                data.WriteValue(value);
            }
            SetDirty();
            return true;
        }

        public void SetDirty ()
        {
            Dirty = true;
        }

        public void AddDataListener(ObservedObject obj, DataEvent evt)
        {
            if (Events.TryGetValue(obj, out var @event)) {
                @event += evt;
                Events[obj] = @event;
            }
            else {
                Events[obj] = evt;
            }
        }

        public void AddDataListener(string path, DataEvent evt) => AddDataListener(GetData(path), evt);

        public void AddDataListener(DataPath path, DataEvent evt) => AddDataListener(GetData(path), evt);

        public void RemoveDataListener(ObservedObject obj, DataEvent evt)
        {
            if (Events.TryGetValue(obj, out var @event)) {
                @event -= evt;
                Events[obj] = @event;
            }
        }

        public void RemoveDataListener(string path, DataEvent evt) => RemoveDataListener(GetData(path), evt);

        public void RemoveDataListener(DataPath path, DataEvent evt) => RemoveDataListener(GetData(path), evt);

        public void ClearDataListener(ObservedObject obj)
        {
            Events.Remove(obj);
        }

        public void ClearDataListener(string path) => ClearDataListener(GetData(path));

        public void ClearDataListener(DataPath path) => ClearDataListener(GetData(path));

        public void ClearAllListener ()
        {
            Events.Clear();
        }

        /// <summary>
        /// clean all data dirty state, and notify listeners of data which dirtied
        /// </summary>
        public void Flush(bool force = false)
        {
            if (!Dirty && !force) return;
            Dirty = false;

            _eventTriggers.Clear();

            void Operation(ObservedObject data)
            {
                if (!data.Dirty) return;
                data.Dirty = false;
                _eventTriggers.Add(data);
            }

            Main.IteratesAllChildElements(Operation, false);

            foreach (var d in _eventTriggers) {
                try {
                    TriggerDataEvent(d);
                }
                catch (Exception e) {
                    Debug.LogError(e.ToString());
                }
            }

            onFlushed?.Invoke();
        }

        public void UpdatePathMapping ()
        {
            var keys = PathMapper.Keys.ToArray();
            foreach (var key in keys) {
                var data = GetData(key);
                if (data == null) {
                    PathMapper.Remove(key);
                }
                else {
                    PathMapper[key] = data;
                }
            }
        }

        internal void TriggerDataEvent(ObservedObject obj)
        {
            if (Events.TryGetValue(obj, out var evt)) {
                evt?.Invoke();
            }
        }
        
    }
}
