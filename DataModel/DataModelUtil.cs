using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace NilanToolkit.DataModel {
    public static class DataModelUtil {
        public static void IteratesAllChildElements(this ObservedObject data, DataOperation operation, bool ignoreCollection)
        {
            switch (data) {
            case IEnumerable<ObservedObject> enumerator: {
                foreach (var child in enumerator) {
                    IteratesAllChildElements(child, operation, ignoreCollection);
                }

                if (!ignoreCollection) {
                    operation?.Invoke(data);
                }
                break;
            }
            default: {
                operation?.Invoke(data);
                break;
            }
            }
        }

        public static ObservedObject DeepSearchData(this ObservedObject root, DataPath path, DataOperation operation = null)
        {
            var stack = new Stack<string>(path.SubPaths);
            var d = root;
            while (stack.Count > 0) {
                var p = stack.Pop();
                if (d is IObservedCollection collection) {
                    operation?.Invoke(d);
                    d = collection.GetItem(p);
                }
                else {
                    Debug.LogWarning("path invalid");
                    return null;
                }
            }

            operation?.Invoke(d);
            return d;
        }

        public static ObservedObject DeepSearchData(this ObservedObject root, string path, DataOperation operation) => DeepSearchData(root, DataPath.Create(path), operation);

        public static T DeepGetValue<T>(this ObservedObject root, DataPath path)
        {
            var data = DeepSearchData(root, path, null);
            if (data != null) {
                return (T) data.value;
            }
            else {
                return default;
            }
        }

        public static bool CheckDataTypeMatch<T>(this ObservedObject data)
        {
            return typeof (T) != data.value.GetType();
        }

        public static object GetValue(this ObservedObject obj)
        {
            return obj.value;
        }

        public static T GetValue<T>(this ObservedObject obj)
        {
            return (T) obj.value;
        }

        public static void SetValue(this ObservedObject obj, object value)
        {
            obj.WriteValue(value);
        }

        public static bool DeepSetValue<T>(this ObservedObject root, T value, DataPath path, bool setParentDirty = true)
        {
            var data = DeepSearchData(root, path, d => {
                if (setParentDirty) d.SetDirty();
            });
            if (data != null) {
                if (typeof (T) != data.value.GetType()) {
                    Debug.LogError("value type not match");
                    return false;
                }
                data.WriteValue(value);
                return true;
            }
            else {
                return false;
            }
        }

        public static ObservedObject Parse(string json)
        {
            var jsonData = JsonMapper.ToObject(json);
            return Parse(jsonData);
        }

        public static ObservedObject Parse(JsonData jsonData)
        {
            switch (jsonData.Type) {
            case JsonType.Object: {
                var dict = new ObservedDictionary();
                foreach (var pair in jsonData.Object) {
                    var child = Parse(pair.Value);
                    dict.Add(pair.Key, child);
                }
                return dict;
            }
            case JsonType.Array: {
                var arr = new ObservedArray();
                for (int i = 0; i < jsonData.Count; i++) {
                    var child = Parse(jsonData[i]);
                    arr.Add(child);
                }
                return arr;
            }
            case JsonType.String: {
                return new ObservedObject<string>(jsonData.ToJson());
            }
            case JsonType.Int: {
                return new ObservedObject<int>((int) jsonData);
            }
            case JsonType.Long: {
                return new ObservedObject<long>((long) jsonData);
            }
            case JsonType.Double: {
                return new ObservedObject<double>((double) jsonData);
            }
            case JsonType.Boolean:
                return new ObservedObject<bool>((bool) jsonData);
            }

            return null;
        }

        public static int FindIndex(this ObservedObject data, Predicate<ObservedObject> predicate)
        {
            if (data is ObservedArray arr) {
                return arr.FindIndex(predicate);
            }
            else {
                Debug.LogError("data is not an array");
                return -1;
            }
        }

    }
}
