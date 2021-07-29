using System.Collections.Generic;
using System.IO;
using System.Text;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

namespace NilanToolkit.ConfigTool.Editor
{
    public class ConfigSettingProvider : SettingsProvider
    { 
        private static GUIStyle headerLabelStyle;

        [SettingsProvider]
        private static SettingsProvider GetSettingsProvider()
        {
            return new ConfigSettingProvider("Excel/Config Tool", SettingsScope.Project);
        }

        public ConfigSettingProvider(string path, SettingsScope scope) : base(path, scope)
        { }

        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);
            if (headerLabelStyle == null)
                headerLabelStyle = new GUIStyle(EditorStyles.boldLabel);

            DrawSeparatorLine("Path Setting");
            DrawSelectPathField(" < Excel Dir > : ", ref ConfigSettings.asset.ExcelFloder);
            DrawSelectPathField(" < Json Path > : ", ref ConfigSettings.asset.JsonFilePath);
            DrawSelectPathField(" < Lua Path > : ", ref ConfigSettings.asset.LuaDataEntryPath);
            DrawSelectPathField(" < CSharp Path > : ", ref ConfigSettings.asset.CSharpDataEntryPath);
            DrawSelectPathField(" < Bytes Path > : ", ref ConfigSettings.asset.BytesFilePath);

            DrawSeparatorLine("Config Generator");

            EditorGUILayout.BeginHorizontal();
            DrawGenerateButton("Generate ( Lua,\n Byte,Json,C# )", TranslatorExcelConfigs);
            DrawGenerateButton("Gen Excel\n Mapping File", WriteExcelNameToPath);
            DrawGenerateButton("Check Excel\n String_Table", CheckStringKeyUniqueness);

            if (GUILayout.Button(" Save Settings ", GUILayout.Width(120), GUILayout.Height(50)))
            {
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.EndHorizontal();

            DrawSeparatorLine("");
        }

        public static void DrawSeparatorLine(string title, int space = 5)
        {
            EditorGUILayout.Separator();
            if (!string.IsNullOrEmpty(title))
            {
                EditorGUILayout.LabelField(title, headerLabelStyle);
            }
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), EditorStyles.label.normal.textColor);
            GUILayout.Space(space);
        }

        private static void DrawSelectPathField(string name, ref string selectPath)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(160));
            GUILayout.Label(selectPath, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                var path = Application.dataPath;
                if (!string.IsNullOrEmpty(selectPath))
                    path = PathUtils.UnityRelativePathToAbsolutePath(selectPath);
                var newPath = EditorUtility.OpenFolderPanel("Select", path, "");
                if (!string.IsNullOrEmpty(newPath) && newPath != selectPath)
                {
                    selectPath = PathUtils.AbsolutePathToUnityRelativePath(newPath);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawGenerateButton(string name, System.Action call)
        {
            if (GUILayout.Button(name, GUILayout.Width(120), GUILayout.Height(50)))
                call?.Invoke();
        }

        private static void TranslatorExcelConfigs()
        {
            TranslatorExcelConfigs(true);
        }

        private static void TranslatorExcelConfigs(bool showDialog, int readMask = 0x7FFFFFFF)
        {
            if (showDialog) EditorUtility.DisplayProgressBar("Translator Excel Configs", "Read Excel", 0f);

            var excelSheets = ReadAllExcelConfigs(showDialog);

            int count = excelSheets.Count;
            int index = 1;
            foreach (var excelSheet in excelSheets.Values)
            {
                if (excelSheet.Name.StartsWith("#")) continue;
                if (excelSheet.Name.StartsWith("Wps")) continue;// wps builtin hidden worksheet
                var translator = new TranslatorTable(excelSheet, readMask);

                var genFileName = translator.sheetName;

                //byte
                string bytePath = Path.Combine(PathUtils.UnityRelativePathToAbsolutePath(ConfigSettings.asset.BytesFilePath), genFileName + ".bytes");
                if (!string.IsNullOrEmpty(bytePath)) File.WriteAllBytes(bytePath, translator.ToDataEntryBytes());

                //json
                string jsonPath = Path.Combine(PathUtils.UnityRelativePathToAbsolutePath(ConfigSettings.asset.JsonFilePath), genFileName + ".json");
                if (!string.IsNullOrEmpty(jsonPath)) File.WriteAllBytes(jsonPath, Encoding.UTF8.GetBytes(translator.ToJson()));

                //lua
                string luaPath = Path.Combine(PathUtils.UnityRelativePathToAbsolutePath(ConfigSettings.asset.LuaDataEntryPath), genFileName + ".lua");
                if (!string.IsNullOrEmpty(luaPath)) File.WriteAllBytes(luaPath, Encoding.UTF8.GetBytes(translator.ToLuaTable()));

                //c#
                string csharpPath = Path.Combine(PathUtils.UnityRelativePathToAbsolutePath(ConfigSettings.asset.CSharpDataEntryPath), genFileName + ".cs");
                if (!string.IsNullOrEmpty(csharpPath)) File.WriteAllBytes(csharpPath, Encoding.UTF8.GetBytes(translator.ToDataEntryClass()));

                if (showDialog)
                {
                    float prog = index * 1f / count;
                    string content = $"Translate Excel:【{translator.sheetName}】";
                    EditorUtility.DisplayProgressBar("Translator Excel Configs", content, prog);
                    index++;
                }
            }

            if (showDialog)
            {
                EditorUtility.DisplayProgressBar("Translator Excel Configs", "Done!", 1);
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Translator Excel Configs", "Done！", "OK");
            }
        }

        private static Dictionary<string, ExcelWorksheet> ReadAllExcelConfigs(bool showDialog)
        {
            var excelSheets = ExcelTranslatorUtility.ReadALLExcelSheets(PathUtils.UnityRelativePathToAbsolutePath(ConfigSettings.asset.ExcelFloder), (excelName, prog) =>
            {
                if (showDialog)
                {
                    string content = $"Read Excel:【{excelName}】 ...";
                    EditorUtility.DisplayProgressBar("Read Excel", content, 1f * prog);
                }
            });
            if (showDialog) EditorUtility.ClearProgressBar();
            return excelSheets;
        }

        private static void WriteExcelNameToPath()
        {
            ExcelTranslatorUtility.WriteExcelNameToPath(ConfigSettings.asset.ExcelFloder, (excelName, prog) =>
            {
                string content = $"Read Excel:【{excelName}】 ...";
                EditorUtility.DisplayProgressBar("Gen Excel Mapping File...", content, 1f * prog);
            });
            EditorUtility.DisplayProgressBar("Gen Excel Mapping File", "Done!", 1);
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Gen Excel Mapping File", "Done！", "OK");
        }

        private static void CheckStringKeyUniqueness()
        {
            CheckExcelKeyUniqueness("string_cfg");
            // CheckExcelNotEmptyCell("string_cfg");
        }

        private static void CheckExcelKeyUniqueness(string configName)
        {
            var excelSheet = ExcelTranslatorUtility.ReadExcelSheet(ConfigSettings.asset.ExcelFloder, configName);
            var checkIDMap = new Dictionary<string, int>();

            var table = new TranslatorTable(excelSheet, 0x7FFFFFFF);
            for (int i = 0; i < table.validRowCount; i++)
            {
                string id = table.ID(i);
                checkIDMap.TryGetValue(id, out var count);
                count++;
                checkIDMap[id] = count;
            }

            bool result = true;
            foreach (var configIDCount in checkIDMap)
            {
                if (configIDCount.Value > 1)
                {
                    Debug.LogErrorFormat("ID = {0} not alone ！，already exit {1} ！", configIDCount.Key, configIDCount.Value);
                    result = false;
                }
            }
            if (result)
            {
                Debug.LogFormat("the Id = {0} Config Excel ...", configName);
            }
        }
    }
}
