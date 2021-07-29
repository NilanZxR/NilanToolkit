using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OfficeOpenXml;
using UnityEngine.UIElements;

namespace NilanToolKit.ConfigTool
{
    public class ConfigSettings : ScriptableObject
    {
        public const string SETTING_PATH = "Assets/Editor/ConfigSettings.asset";

        private static ConfigSettings s_asset;

        public static ConfigSettings asset
        {
            get
            {
                if (s_asset == null)
                {
                    if (!Directory.Exists(Application.dataPath + "/Editor"))//
                        Directory.CreateDirectory(Application.dataPath + "/Editor");
                    s_asset = AssetDatabase.LoadAssetAtPath<ConfigSettings>(SETTING_PATH);
                    if (s_asset == null)
                    {
                        s_asset = ScriptableObject.CreateInstance<ConfigSettings>();
                        AssetDatabase.CreateAsset(asset, SETTING_PATH);
                    }
                }
                return s_asset;
            }
        }

        public string ExcelFloder;

        public string JsonFilePath;

        public string LuaDataEntryPath;

        public string CSharpDataEntryPath;

        public string BytesFilePath;



    }
}
