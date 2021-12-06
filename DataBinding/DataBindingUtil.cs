using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace NilanToolkit.DataBinding {
    public static class DataBindingUtil {
        public static void IteratesAllChildElements(this BindingObject data, Action<BindingObject> operation, bool ignoreCollection)
        {
            switch (data) {
            case IEnumerable<BindingObject> enumerator: {
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

        public static BindingObject DeepSearchData(this BindingObject root, DataPath path, Action<BindingObject> operation = null)
        {
            var hierarchy = path.GetHierarchy();
            var curr = root;
            var depth = 0;
            for (int i = 0; i < hierarchy.Length; i++) {
                if (curr is IDataBindingCollection collection) {
                    operation?.Invoke(curr);
                    curr = collection.GetItem(hierarchy[i]);
                }
                else {
                    Debug.LogWarning("path invalid");
                    return null;
                }
            }

            operation?.Invoke(curr);
            return curr;
        }

        public static T DeepGetValue<T>(this BindingObject root, DataPath path)
        {
            var data = DeepSearchData(root, path);
            if (data != null) {
                return (T) data.value;
            }
            else {
                return default;
            }
        }

        public static bool IsDataTypeMatch<T>(this BindingObject data)
        {
            return typeof (T) != data.value.GetType();
        }

        public static object GetValue(this BindingObject target)
        {
            return target.value;
        }

        public static T GetValue<T>(this BindingObject target)
        {
            return (T) target.value;
        }

        public static void SetValue(this BindingObject target, object value)
        {
            target.WriteValue(value);
        }

        public static bool DeepSetValue<T>(this BindingObject target, T value, DataPath path, bool setParentDirty = true)
        {
            var data = DeepSearchData(target, path, d => {
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

        public static BindingObject Parse(string json)
        {
            var jsonData = JsonMapper.ToObject(json);
            return Parse(jsonData);
        }

        public static BindingObject Parse(JsonData jsonData)
        {
            switch (jsonData.Type) {
            case JsonType.Object: {
                var dict = new BindingDictionary();
                foreach (var pair in jsonData.Object) {
                    var child = Parse(pair.Value);
                    dict.Add(pair.Key, child);
                }
                return dict;
            }
            case JsonType.Array: {
                var arr = new BindingList();
                for (int i = 0; i < jsonData.Count; i++) {
                    var child = Parse(jsonData[i]);
                    arr.Add(child);
                }
                return arr;
            }
            case JsonType.String: {
                return new BindingObject<string>(jsonData.ToJson());
            }
            case JsonType.Int: {
                return new BindingObject<int>((int) jsonData);
            }
            case JsonType.Long: {
                return new BindingObject<long>((long) jsonData);
            }
            case JsonType.Double: {
                return new BindingObject<double>((double) jsonData);
            }
            case JsonType.Boolean:
                return new BindingObject<bool>((bool) jsonData);
            }

            return null;
        }

    }
}
