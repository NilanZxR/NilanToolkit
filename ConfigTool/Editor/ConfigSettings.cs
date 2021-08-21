using System.IO;
using UnityEditor;
using UnityEngine;

namespace NilanToolkit.ConfigTool.Editor {
public class ConfigSettings {
    public static string excelFolderPath;

    public static string jsonFilePath;

    public static string luaDataEntryPath;

    public static string cSharpDataEntryPath;

    public static string bytesFilePath;

    public static void Load() {
        excelFolderPath = EditorPrefs.GetString("CONFIGTOOL_ExcelFolderPath");
        jsonFilePath = EditorPrefs.GetString("CONFIGTOOL_JsonFilePath");
        luaDataEntryPath = EditorPrefs.GetString("CONFIGTOOL_JsonFilePath");
        cSharpDataEntryPath = EditorPrefs.GetString("CONFIGTOOL_CSharpDataEntryPath");
        bytesFilePath = EditorPrefs.GetString("CONFIGTOOL_BytesFilePath");
    }

    public static void Save() {
        EditorPrefs.SetString("CONFIGTOOL_ExcelFolderPath", excelFolderPath);
        EditorPrefs.SetString("CONFIGTOOL_JsonFilePath", jsonFilePath);
        EditorPrefs.SetString("CONFIGTOOL_LuaDataEntryPath", luaDataEntryPath);
        EditorPrefs.SetString("CONFIGTOOL_CSharpDataEntryPath", cSharpDataEntryPath);
        EditorPrefs.SetString("CONFIGTOOL_BytesFilePath", bytesFilePath);
    }
    
}
}