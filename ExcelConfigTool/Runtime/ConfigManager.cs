using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Reflection;
using OfficeOpenXml;

namespace NilanToolKit.ConfigTool
{
    public delegate byte[] ConfigLoadDelegate(string configName);

    public class ConfigManager
    {

        private Dictionary<string, DataEntryCache> mDataEntryCaches = new Dictionary<string, DataEntryCache>();

        private ConfigLoadDelegate m_ConfigLoadMethod;

        public ConfigManager(ConfigLoadDelegate method) {
            SetLoadByteMethod(method);
        }

        public void SetLoadByteMethod(ConfigLoadDelegate method)
        {
            m_ConfigLoadMethod = method;
        }

        private byte[] LoadConfigBytes(string configName)
        {
            byte[] bytes = null;
            bytes = m_ConfigLoadMethod?.Invoke(configName);
            return bytes;
        }

        public T GetConfig<T>(string id) where T : DataEntryBase
        {
            Type type = typeof(T);

            var fi = type.GetField("sheetName", BindingFlags.Static| BindingFlags.Public);

            string configName = fi.GetValue(type).ToString();

            return GetTableCache(configName, type).GetEntry<T>(id);
        }

        public DataEntryCache GetTableCache<T>() where T : DataEntryBase
        {
            Type type = typeof(T);

            var fi = type.GetField("sheetName", BindingFlags.Static | BindingFlags.Public);

            string configName = fi.GetValue(type).ToString();

            return GetTableCache(configName, type);
        }

        public DataEntryCache GetTableCache(string configName, Type type)
        {
            DataEntryCache entryCache = null;

            if (!mDataEntryCaches.TryGetValue(configName, out entryCache))
            {
                byte[] bytes = LoadConfigBytes(configName);

                entryCache = TranslatorTable.ToTableCache(bytes, type);

                mDataEntryCaches.Add(configName, entryCache);
            }
            return entryCache;
        }

        public byte[] GetLuaTableBytes(string configName)
        {
            byte[] bytes = LoadConfigBytes(configName);

            string luaString = TranslatorTable.ToLuaTable(bytes);

            return UTF8Encoding.UTF8.GetBytes(luaString);
        }

        public string GetJsonDataTable(string configName)
        {
            byte[] bytes = LoadConfigBytes(configName);

            return TranslatorTable.ToJson(bytes);
        }

        public void UnloadConfig(string configName)
        {
            mDataEntryCaches.Remove(configName);
        }
    }

}