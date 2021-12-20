using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NilanToolkit.ConfigTool.Editor
{
    public class ConfigSettingProvider : SettingsProvider {
        
        private static GUIStyle headerLabelStyle;

        [SettingsProvider]
        private static SettingsProvider GetSettingsProvider()
        {
            return new ConfigSettingProvider("Excel/Config Tool", SettingsScope.Project);
        }

        public ConfigSettingProvider(string path, SettingsScope scope) : base(path, scope)
        { }

        public override void OnActivate(string searchContext, VisualElement rootElement) {
            base.OnActivate(searchContext, rootElement);
            ConfigSettings.Load();
        }

        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);
            if (headerLabelStyle == null)
                headerLabelStyle = new GUIStyle(EditorStyles.boldLabel);

            var isExcelPathNotSet = string.IsNullOrEmpty(ConfigSettings.excelFolderPath);
            if (isExcelPathNotSet) {
                EditorGUILayout.HelpBox("未设置Excel目录", MessageType.Error);
            }

            DrawSeparatorLine("数据路径");
            
            DrawSelectPathField("Excel目录", ref ConfigSettings.excelFolderPath);
            
            DrawSeparatorLine("输出路径");
            DrawSelectPathField("Json目录", ref ConfigSettings.jsonFilePath);
            DrawSelectPathField("Lua目录", ref ConfigSettings.luaDataEntryPath);
            DrawSelectPathField("C#目录", ref ConfigSettings.cSharpClassPath);
            DrawSelectPathField("Bytes目录", ref ConfigSettings.bytesFilePath);
            
            
            DrawSeparatorLine("输出设置");
            ConfigSettings.clearDir = EditorGUILayout.Toggle("生成时清理目录", ConfigSettings.clearDir);
            ConfigSettings.cSharpClassNamespace = EditorGUILayout.TextField("[C#]命名空间", ConfigSettings.cSharpClassNamespace);
            ConfigSettings.cSharpClassNameFormatter = EditorGUILayout.TextField("[C#]类名格式", ConfigSettings.cSharpClassNameFormatter);
            
            if (GUILayout.Button("保存设置")) {
                ConfigSettings.Save();
            }
            
            DrawSeparatorLine("生成器");
            
            // if (GUILayout.Button(" Save", GUILayout.Width(120), GUILayout.Height(50)))
            // {
            //     ConfigSettings.Save();
            // }

            if (!isExcelPathNotSet) {
                DrawGenerateButton("一键生成!", TranslatorExcelConfigs);
            }
            
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
                    path = PathUtils.ToAbsolutePath(selectPath);
                var newPath = EditorUtility.OpenFolderPanel("Select", path, "");
                if (!string.IsNullOrEmpty(newPath) && newPath != selectPath)
                {
                    selectPath = PathUtils.ToUnityRelativePath(newPath);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawGenerateButton(string name, Action call)
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

            if (ConfigSettings.clearDir) {
                if(!string.IsNullOrEmpty(ConfigSettings.bytesFilePath)) 
                    ClearDir(PathUtils.ToAbsolutePath(ConfigSettings.bytesFilePath));
                if(!string.IsNullOrEmpty(ConfigSettings.jsonFilePath)) 
                    ClearDir(PathUtils.ToAbsolutePath(ConfigSettings.jsonFilePath));
                if(!string.IsNullOrEmpty(ConfigSettings.luaDataEntryPath)) 
                    ClearDir(PathUtils.ToAbsolutePath(ConfigSettings.luaDataEntryPath));
                if(!string.IsNullOrEmpty(ConfigSettings.cSharpClassPath)) 
                    ClearDir(PathUtils.ToAbsolutePath(ConfigSettings.cSharpClassPath));
            }
            
            var excelSheets = ReadAllExcelConfigs(showDialog);

            int count = excelSheets.Count;
            int index = 1;
            foreach (var excelSheet in excelSheets.Values)
            {
                if (excelSheet.Name.StartsWith("#")) continue;//Remark worksheet, can be removed
                if (excelSheet.Name.StartsWith("Wps")) continue;// wps builtin hidden worksheet, can be removed if is not wps excel
                var translator = new TranslatorTable(excelSheet, readMask);

                var genFileName = translator.sheetName;

                //byte
                if (!string.IsNullOrEmpty(ConfigSettings.bytesFilePath)) {
                    string bytePath = Path.Combine(PathUtils.ToAbsolutePath(ConfigSettings.bytesFilePath), genFileName + ".bytes");
                    File.WriteAllBytes(bytePath, TranslatorTableConverter.ToDataBlockBytes(translator));
                }
                
                //json
                if (!string.IsNullOrEmpty(ConfigSettings.jsonFilePath)) {
                    string jsonPath = Path.Combine(PathUtils.ToAbsolutePath(ConfigSettings.jsonFilePath), genFileName + ".json");
                    var json = TranslatorTableConverter.ToJson(translator);
                    File.WriteAllBytes(jsonPath, Encoding.UTF8.GetBytes(json));
                }
                
                //lua
                if (!string.IsNullOrEmpty(ConfigSettings.jsonFilePath)) {
                    string luaPath = Path.Combine(PathUtils.ToAbsolutePath(ConfigSettings.luaDataEntryPath), genFileName + ".lua");
                    var lua = TranslatorTableConverter.ToLuaTable(translator);
                    File.WriteAllBytes(luaPath, Encoding.UTF8.GetBytes(lua));
                }

                //c#
                if (!string.IsNullOrEmpty(ConfigSettings.cSharpClassPath)) {
                    string csharpPath = Path.Combine(PathUtils.ToAbsolutePath(ConfigSettings.cSharpClassPath), genFileName + ".cs");
                    var info = new CSharpClassFileGenerateInfo(ConfigSettings.cSharpClassNamespace, ConfigSettings.cSharpClassNameFormatter);
                    var csFile = TranslatorTableConverter.ToDataBlockCSharpFile(translator, info);
                    File.WriteAllBytes(csharpPath, Encoding.UTF8.GetBytes(csFile));
                }

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

        private static void ClearDir(string path) {
            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists) {
                return;
            }
            var fileInfos = directoryInfo.GetFiles();
            if (fileInfos.Length == 0) return;

            var sb = new StringBuilder();
            foreach (var fileInfo in fileInfos) {
                sb.AppendLine(fileInfo.Name);
            }

            if (EditorUtility.DisplayDialog("是否删除？", sb.ToString(), "确定", "取消")) {
                foreach (var file in fileInfos) {
                    file.Delete();
                }
            }
        }

        private static Dictionary<string, ExcelWorksheet> ReadAllExcelConfigs(bool showDialog)
        {
            var excelSheets = ReadAllExcelSheets(
                PathUtils.ToAbsolutePath(ConfigSettings.excelFolderPath), 
                (excelName, prog) => {
                    if (showDialog)
                    {
                        string content = $"Read Excel:【{excelName}】 ...";
                        EditorUtility.DisplayProgressBar("Read Excel", content, 1f * prog);
                    }
                }
                );
            if (showDialog) EditorUtility.ClearProgressBar();
            return excelSheets;
        }
        
        public static Dictionary<string, ExcelWorksheet> ReadAllExcelSheets(string excelFolder,  Action<string, float> readCallback = null)
        {
            var ret = new Dictionary<string, ExcelWorksheet>();
            if (!Directory.Exists(excelFolder)) return ret;
            
            var dirInfo = Directory.CreateDirectory(excelFolder);
            var fileInfos = dirInfo.GetFiles().Where(i=>i.Name.EndsWith(".xlsx"));
            var count = fileInfos.Count();
            var index = 0;
            foreach (var fileInfo in fileInfos) {
                if (fileInfo.Name.EndsWith(".xlsx"))
                {
                    var package = new ExcelPackage(fileInfo);
                    var sheets = package.Workbook.Worksheets;
                    foreach (var sheet in sheets) {
                        ret.Add(sheet.Name, sheet);
                    }
                }
                readCallback?.Invoke(fileInfo.Name, (index + 1) * 1f / count);
                index++;
            }
            return ret;
        }
    }
}
