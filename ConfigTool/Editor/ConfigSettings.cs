using System.IO;
using UnityEditor;
using UnityEngine;

namespace NilanToolkit.ConfigTool.Editor {
public class ConfigSettings {
    public static string excelFolderPath;

    public static string jsonFilePath;

    public static string luaDataEntryPath;

    public static string cSharpClassNamespace;

    public static string cSharpClassNameFormatter;

    public static string cSharpClassPath;

    public static string bytesFilePath;
    
    public static bool clearDir;

    public static void Load() {
        excelFolderPath = EditorPrefs.GetString(PrefName(nameof(excelFolderPath)));
        jsonFilePath = EditorPrefs.GetString(PrefName(nameof(jsonFilePath)));
        luaDataEntryPath = EditorPrefs.GetString(PrefName(nameof(luaDataEntryPath)));
        cSharpClassNamespace = EditorPrefs.GetString(PrefName(nameof(cSharpClassNamespace)), "GeneratedData");
        cSharpClassNameFormatter = EditorPrefs.GetString(PrefName(nameof(cSharpClassNameFormatter)), "{0}");
        cSharpClassPath = EditorPrefs.GetString(PrefName(nameof(cSharpClassPath)));
        bytesFilePath = EditorPrefs.GetString(PrefName(nameof(bytesFilePath)));
        clearDir = EditorPrefs.GetBool(PrefName(nameof(clearDir)));
    }

    public static void Save() {
        EditorPrefs.SetString(PrefName(nameof(excelFolderPath)), excelFolderPath);
        EditorPrefs.SetString(PrefName(nameof(jsonFilePath)), jsonFilePath);
        EditorPrefs.SetString(PrefName(nameof(luaDataEntryPath)), luaDataEntryPath);
        EditorPrefs.SetString(PrefName(nameof(cSharpClassNamespace)), cSharpClassNamespace);
        EditorPrefs.SetString(PrefName(nameof(cSharpClassPath)), cSharpClassPath);
        EditorPrefs.SetString(PrefName(nameof(bytesFilePath)), bytesFilePath);
        EditorPrefs.SetBool(PrefName(nameof(clearDir)), clearDir);
    }

    private static string PrefName(string n) {
        return $"CONFIGTOOL_{n}";
    }
    
}
}