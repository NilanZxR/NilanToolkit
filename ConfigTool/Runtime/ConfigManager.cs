using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NilanToolkit.ConfigTool {
    public delegate byte[] ConfigLoadDelegate(string configName);

    public class ConfigManager {

        private Dictionary<string, DataSheet> sheets = new Dictionary<string, DataSheet>();

        private ConfigLoadDelegate configLoader;

        public ConfigManager(ConfigLoadDelegate method) {
            SetLoadByteMethod(method);
        }

        public void SetLoadByteMethod(ConfigLoadDelegate method) {
            configLoader = method;
        }

        public T Get<T>(string id) where T : DataEntryBase {
            var type = typeof (T);
            var sheetName = GetSheetName(type);
            return GetDataSheet(sheetName, type).GetEntry<T>(id);
        }

        public T Find<T>(Predicate<T> predicate) where T : DataEntryBase {
            var type = typeof (T);
            var sheetName = GetSheetName(type);
            var tableCache = GetDataSheet(sheetName, type);
            foreach (var entry in tableCache.Sheet.Values) {
                var typedEntry = entry as T;
                if (predicate.Invoke(typedEntry)) {
                    return typedEntry;
                }
            }
            return null;
        }

        public DataSheet GetDataSheet<T>() where T : DataEntryBase {
            var type = typeof (T);
            var sheetName = GetSheetName(type);
            return GetDataSheet(sheetName, type);
        }

        public DataSheet GetDataSheet(string configName, Type type) {
            if (!sheets.TryGetValue(configName, out var entryCache)) {
                byte[] bytes = LoadConfigBytes(configName);
                entryCache = TranslatorTable.ToTableCache(bytes, type);
                sheets.Add(configName, entryCache);
            }
            return entryCache;
        }

        public byte[] GetLuaTableBytes(string configName) {
            byte[] bytes = LoadConfigBytes(configName);
            string luaString = TranslatorTable.ToLuaTable(bytes);
            return UTF8Encoding.UTF8.GetBytes(luaString);
        }

        public string GetJsonDataTable(string configName) {
            byte[] bytes = LoadConfigBytes(configName);
            return TranslatorTable.ToJson(bytes);
        }

        public void UnloadConfig(string configName) {
            sheets.Remove(configName);
        }

        private byte[] LoadConfigBytes(string configName) {
            var bytes = configLoader?.Invoke(configName);
            return bytes;
        }

        private string GetSheetName(Type type) {
            var fi = type.GetField("sheetName", BindingFlags.Static | BindingFlags.Public);
            string configName = fi.GetValue(type).ToString();
            return configName;
        }

    }

}
