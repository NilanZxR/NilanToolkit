using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NilanToolkit.ConfigTool {
    public delegate byte[] ConfigLoadDelegate(string configName);

    public class ConfigManager {

        private Dictionary<Type, IDataSheet> sheets = new Dictionary<Type, IDataSheet>();

        private ConfigLoadDelegate _configLoader;

        public ConfigManager(ConfigLoadDelegate method) {
            SetLoadByteMethod(method);
        }

        public void SetLoadByteMethod(ConfigLoadDelegate method) {
            _configLoader = method;
        }

        public T Get<T>(string key) where T : DataBlockBase {
            var sheet = GetOrLoadDataSheet<T>();
            return sheet[key];
        }

        public IEnumerable<T> GetAll<T>() where T : DataBlockBase {
            var sheet = GetOrLoadDataSheet<T>();
            return sheet.Blocks;
        }

        public T Find<T>(Predicate<T> predicate) where T : DataBlockBase {
            var tableCache = GetOrLoadDataSheet<T>();
            foreach (var pair in tableCache) {
                if (predicate.Invoke(pair.Value)) {
                    return pair.Value;
                }
            }
            return null;
        }

        public bool TryGetEntry<T>(string key, out T value) where T : DataBlockBase {
            var sheet = GetOrLoadDataSheet<T>();
            return sheet.TryGetValue(key, out value);
        }

        public IDataSheet<T> GetOrLoadDataSheet<T>() where T : DataBlockBase {
            var type = typeof(T);
            if (!sheets.TryGetValue(type, out var dataSheet)) {
                var sheetName = GetSheetName(type);
                byte[] bytes = LoadConfigBytes(sheetName);
                dataSheet = TranslatorTable.ToTableCache<T>(bytes);
                sheets.Add(type, dataSheet);
            }
            return dataSheet as IDataSheet<T>;
        }

        public void UnloadConfig<T>() {
            sheets.Remove(typeof(T));
        }

        private byte[] LoadConfigBytes(string configName) {
            var bytes = _configLoader?.Invoke(configName);
            return bytes;
        }

        private string GetSheetName(Type type) {
            var fi = type.GetField("sheetName", BindingFlags.Static | BindingFlags.Public);
            string configName = fi.GetValue(type).ToString();
            return configName;
        }

    }

}
