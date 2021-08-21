using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace NilanToolkit {
/// <summary>
/// file io in unity
/// </summary>
public static class FileUtil {
    public static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

    public static readonly string[] SizeSuffixes = {
        "Bytes",
        "KB",
        "MB",
        "GB",
        "TB",
        "PB",
        "EB",
        "ZB",
        "YB"
    };

    public static string ReadFrom(string path){
        if(path.StartsWith(Application.persistentDataPath)){
            return File.ReadAllText(path);
        }
        if(path.StartsWith(Application.streamingAssetsPath)){
            return ReadFromStreamingAssetPath(path);
        }
        throw new Exception($"try to read from path {path}");
    }

    

    public static string ReadFromStreamingAssetPath(string path){
#if UNITY_ANDROID && !UNITY_EDITOR
        var www = new  UnityWebRequest(path);
        while(!www.isDone){}
        return www.downloadHandler.text;
#else
        return File.ReadAllText(path);
#endif
    }

    public static void Copy(string from,string to,string fileName){
        from = Path.Combine(from,fileName);
        to = Path.Combine(to,fileName);
        File.Copy(from,to,true);
    }

    public static void CopyFromStream(string from, string to, string fileName) {
        from = Path.Combine(from, fileName);
        to = Path.Combine(to, fileName);
        
#if UNITY_ANDROID && !UNITY_EDITOR
        var www = new UnityWebRequest(from);
        while(!www.isDone){}
        FileUtil.WriteAllBytes(to, www.downloadHandler.data);
#else
        File.Copy(from, to, true);   
#endif
    }

    public static void CopyFromStream(string from, string to) {
#if UNITY_ANDROID && !UNITY_EDITOR
        var www = new UnityWebRequest(from);
        while(!www.isDone){}
        WriteAllBytes(to, www.downloadHandler.data);
#else
        File.Copy(from, to, true);   
#endif
    }



    public static void CopyDirectory(string sourcePath, string destinationPath, string fileFilter = "*", bool skipMetaFile = true, bool sync = false) {
        if (sourcePath != destinationPath && Directory.Exists(sourcePath)) {
            var text = ".meta";
            HashSet<string> hashSet = null;
            if (sync) {
                hashSet = new HashSet<string>();
            }
            var directories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
            var array = directories;
            foreach (var text2 in array) {
                var text3 = text2.Replace(sourcePath, destinationPath);
                Directory.CreateDirectory(text3);
                if (sync) {
                    hashSet.Add(text3);
                }
            }
            if (!Directory.Exists(destinationPath)) {
                Directory.CreateDirectory(destinationPath);
            }
            var files = Directory.GetFiles(sourcePath, fileFilter, SearchOption.AllDirectories);
            var array2 = files;
            foreach (var text4 in array2) {
                if (!text4.EndsWith(text) || !skipMetaFile) {
                    var text5 = text4.Replace(sourcePath, destinationPath);
                    File.Copy(text4, text5, true);
                    if (sync) {
                        hashSet.Add(text5);
                    }
                }
            }
            if (sync) {
                directories = Directory.GetDirectories(destinationPath, "*", SearchOption.AllDirectories);
                var array3 = directories;
                foreach (var text6 in array3) {
                    if (!hashSet.Contains(text6) && Directory.Exists(text6)) {
                        Directory.Delete(text6, true);
                    }
                }
                files = Directory.GetFiles(destinationPath, "*", SearchOption.AllDirectories);
                var array4 = files;
                foreach (var text7 in array4) {
                    if ((!text7.EndsWith(text) || !skipMetaFile || !hashSet.Contains(text7.Substring(0, text7.Length - text.Length))) && !hashSet.Contains(text7) && File.Exists(text7)) {
                        File.Delete(text7);
                    }
                }
            }
        }
    }

    public static byte[] ReadAllBytes(string path) {
        try {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!path.Contains("file://"))
            {
                path = "file://" + path;
            }
            var www = new UnityEngine.Networking.UnityWebRequest(path);
            while (!www.isDone){}

            return Encoding.Default.GetBytes(www.downloadHandler.text);
#else
            return File.ReadAllBytes(path);
#endif
        }
        catch (Exception e) {
            Debug.LogError(e.ToString());
        }

        return null;
    }

    public static bool WriteAllBytes(string path, byte[] bytes) {
        try {
            File.WriteAllBytes(path, bytes);
            return true;
        } catch (Exception e) {
            Debug.LogError(e.ToString());
        }
        return false;
    }

    public static string ReadAllText(string path, Encoding encoding = null) {
        try {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!path.Contains("file://"))
            {
                path = "file://" + path;
            }
            var www = new UnityWebRequest(path);
            while (!www.isDone){}

            return www.downloadHandler.text;
#else
            var bytes = File.ReadAllBytes(path);
            return (encoding != null) ? encoding.GetString(bytes) : Utf8NoBom.GetString(bytes);
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"read file error {path}, {e}");
        }

        return null;
    }

    public static List<T> SafeBinaryFormatterReadList<T>(string path) {
        List<T> list = null;
        if (File.Exists(path)) {
            try {
                var binaryFormatter = new BinaryFormatter();
                var fileStream = File.Open(path, FileMode.Open);
                list = ((fileStream.Length != 0) ? ((List<T>)binaryFormatter.Deserialize(fileStream)) : new List<T>());
                fileStream.Close();
            } 
            catch (Exception e) {
                Debug.LogError(e.ToString());
            }
        }
        path += "_temp";
        if (File.Exists(path)) {
            try {
                var binaryFormatter2 = new BinaryFormatter();
                var fileStream2 = File.Open(path, FileMode.Open);
                list = ((fileStream2.Length != 0) ? ((List<T>)binaryFormatter2.Deserialize(fileStream2)) : new List<T>());
                fileStream2.Close();
            } 
            catch (Exception e) {
                Debug.LogError(e.ToString());
            }
        }
        return list ?? new List<T>();
    }

    public static string SafeBinaryFormatterReadText(string path) {
        string result = null;
        if (File.Exists(path)) {
            try {
                var binaryFormatter = new BinaryFormatter();
                var fileStream = File.Open(path, FileMode.Open);
                result = (string)binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
            } 
            catch (Exception e) {
                Debug.LogError(e.ToString());
            }
        }
        path += "_temp";
        if (File.Exists(path)) {
            try {
                var binaryFormatter2 = new BinaryFormatter();
                var fileStream2 = File.Open(path, FileMode.Open);
                result = (string)binaryFormatter2.Deserialize(fileStream2);
                fileStream2.Close();
                return result;
            } catch (Exception) {
                return result;
            }
        }
        return result;
    }

    public static bool SafeBinaryFormatterWrite(string path, object contents) {
        var text = path + "_temp";
        try {
            var fileStream = File.Create(text);
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, contents);
            fileStream.Close();
            if (File.Exists(path)) {
                File.Delete(path);
            }
            File.Move(text, path);
            return true;
        } catch (Exception e) {
            Debug.LogError(e.ToString());
        }
        return false;
    }

    public static bool SafeWriteAllText(string path, string contents) {
        var text = path + "_temp";
        try {
            File.WriteAllText(text, contents);
            if (File.Exists(path)) {
                File.Delete(path);
            }
            File.Move(text, path);
            return true;
        } catch (Exception e) {
            Debug.LogError(e.ToString());
        }
        return false;
    }

    public static string SafeReadAllText(string path) {
        string text = null;
        if(File.Exists(path)){
            try {
                text = File.ReadAllText(path);
                if (string.IsNullOrEmpty(text)) {
                    return text;
                }
                text = File.ReadAllText(path + "_temp");
                return text;
            } catch (Exception) {
                return text;
            }
        }
        return null;
    }

    public static bool SafeWriteAllBytes(string path, byte[] bytes) {
        var text = path + "_temp";
        try {
            File.WriteAllBytes(text, bytes);
            if (File.Exists(path)) {
                File.Delete(path);
            }
            File.Move(text, path);
            return true;
        } catch (Exception e) {
            Debug.LogError(e.ToString());
        }
        return false;
    }

    public static byte[] SafeReadAllBytes(string path) {
        byte[] array = null;
        try {
            array = File.ReadAllBytes(path);
            if (array.Length != 0) {
                return array;
            }
            array = File.ReadAllBytes(path + "_temp");
            return array;
        } catch (Exception) {
            return array;
        }
    }

    public static bool WriteAllText(string path, string contents, Encoding encoding = null) {
        try {
            var bytes = (encoding != null) ? encoding.GetBytes(contents) : Utf8NoBom.GetBytes(contents);
            File.WriteAllBytes(path, bytes);
            return true;
        } catch (Exception e) {
            Debug.LogError(e.ToString());
        }
        return false;
    }

    public static object ReadObject(string path) {
        FileStream fileStream = null;
        try {
            fileStream = File.OpenRead(path);
            var binaryFormatter = new BinaryFormatter();
            return binaryFormatter.Deserialize(fileStream);
        } 
        catch (Exception e) {
            Debug.LogError(e.ToString());
        }
        finally {
            if (fileStream != null) {
                try {
                    fileStream.Close();
                } catch (Exception e) {
                    Debug.LogError(e.ToString());
                }
            }
        }
        return null;
    }

    public static bool WriteObject(string path, object o) {
        FileStream fileStream = null;
        try {
            fileStream = File.Create(path);
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, o);
            return true;
        } 
        catch (Exception e) {
            Debug.LogError(e.ToString());
        }
        finally {
            if (fileStream != null) {
                try {
                    fileStream.Close();
                } 
                catch (Exception e) {
                    Debug.LogError(e.ToString());
                }
            }
        }
        return false;
    }

    public static bool RenameFile(string path, string newName) {
        try {
            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists) {
                fileInfo.MoveTo(Path.Combine(fileInfo.DirectoryName ?? "", newName));
                return true;
            }
            var directoryInfo = new DirectoryInfo(path);
            if (directoryInfo.Exists) {
                if (directoryInfo.Parent != null) {
                    directoryInfo.MoveTo(Path.Combine(directoryInfo.Parent.FullName, newName));
                    return true;
                }
                else return false;
            }
        } 
        catch (Exception e) {
            Debug.LogError(e.ToString());
        }
        return false;
    }

    public static string ByteArrToString(byte[] content, Encoding encoding = null) {
        return (encoding ?? Utf8NoBom).GetString(content);
    }

    public static byte[] StringToByteArr(string content, Encoding encoding = null) {
        return (encoding ?? Utf8NoBom).GetBytes(content);
    }

    public static string SizeSuffix(long value, int decimalPlaces = 1) {
        if (value < 0) {
            return "-" + SizeSuffix(-value);
        }
        var num = 0;
        decimal num2 = value;
        while (Math.Round(num2, decimalPlaces) >= 1000m) {
            num2 /= 1024m;
            num++;
        }

        var formatter = "{0:n" + decimalPlaces + "} {1}";
        return string.Format(formatter, num2, SizeSuffixes[num]);
    }

    public static bool FileEquals(string file1, string file2) {
        var fileInfo = new FileInfo(file1);
        var fileInfo2 = new FileInfo(file2);
        if (!fileInfo.Exists || !fileInfo2.Exists || fileInfo.Length != fileInfo2.Length) {
            return false;
        }
        var array = File.ReadAllBytes(file1);
        var array2 = File.ReadAllBytes(file2);
        var i = 0;
        for (var num = array.Length; i < num; i++) {
            if (array[i] != array2[i]) {
                return false;
            }
        }
        return true;
    }
}
}