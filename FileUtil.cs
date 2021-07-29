using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace NilanToolKit {
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
        var www = new  WWW(path);
        while(!www.isDone){}
        return www.text;
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
        var www = new WWW(from);
        while(!www.isDone){}
        FileUtil.WriteAllBytes(to, www.bytes);
#else
        File.Copy(from, to, true);   
#endif
    }

    public static void CopyFromStream(string from, string to) {
#if UNITY_ANDROID && !UNITY_EDITOR
        var www = new WWW(from);
        while(!www.isDone){}
        FileUtil.WriteAllBytes(to, www.bytes);
#else
        File.Copy(from, to, true);   
#endif
    }



    public static void CopyDirectory(string sourcePath, string destinationPath, string fileFilter = "*", bool skipMetaFile = true, bool sync = false) {
        if (sourcePath != destinationPath && Directory.Exists(sourcePath)) {
            string text = ".meta";
            HashSet<string> hashSet = null;
            if (sync) {
                hashSet = new HashSet<string>();
            }
            string[] directories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
            string[] array = directories;
            foreach (string text2 in array) {
                string text3 = text2.Replace(sourcePath, destinationPath);
                Directory.CreateDirectory(text3);
                if (sync) {
                    hashSet.Add(text3);
                }
            }
            if (!Directory.Exists(destinationPath)) {
                Directory.CreateDirectory(destinationPath);
            }
            string[] files = Directory.GetFiles(sourcePath, fileFilter, SearchOption.AllDirectories);
            string[] array2 = files;
            foreach (string text4 in array2) {
                if (!text4.EndsWith(text) || !skipMetaFile) {
                    string text5 = text4.Replace(sourcePath, destinationPath);
                    File.Copy(text4, text5, true);
                    if (sync) {
                        hashSet.Add(text5);
                    }
                }
            }
            if (sync) {
                directories = Directory.GetDirectories(destinationPath, "*", SearchOption.AllDirectories);
                string[] array3 = directories;
                foreach (string text6 in array3) {
                    if (!hashSet.Contains(text6) && Directory.Exists(text6)) {
                        Directory.Delete(text6, true);
                    }
                }
                files = Directory.GetFiles(destinationPath, "*", SearchOption.AllDirectories);
                string[] array4 = files;
                foreach (string text7 in array4) {
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
            WWW www = new WWW(path);
            while (!www.isDone){}

            return Encoding.Default.GetBytes(www.text);
        
#endif
            return File.ReadAllBytes(path);
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
            WWW www = new WWW(path);
            while (!www.isDone){}

            return www.text;
#else
            byte[] bytes = File.ReadAllBytes(path);
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
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = File.Open(path, FileMode.Open);
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
                BinaryFormatter binaryFormatter2 = new BinaryFormatter();
                FileStream fileStream2 = File.Open(path, FileMode.Open);
                list = ((fileStream2.Length != 0) ? ((List<T>)binaryFormatter2.Deserialize(fileStream2)) : new List<T>());
                fileStream2.Close();
            } 
            catch (Exception e) {
                Debug.LogError(e.ToString());
            }
        }
        if (list == null) {
            list = new List<T>();
        }
        return list;
    }

    public static string SafeBinartFormatterReadText(string path) {
        string result = null;
        if (File.Exists(path)) {
            try {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = File.Open(path, FileMode.Open);
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
                BinaryFormatter binaryFormatter2 = new BinaryFormatter();
                FileStream fileStream2 = File.Open(path, FileMode.Open);
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
        string text = path + "_temp";
        try {
            FileStream fileStream = File.Create(text);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
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
        string text = path + "_temp";
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
        string text = path + "_temp";
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
            byte[] bytes = (encoding != null) ? encoding.GetBytes(contents) : Utf8NoBom.GetBytes(contents);
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
            BinaryFormatter binaryFormatter = new BinaryFormatter();
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
            BinaryFormatter binaryFormatter = new BinaryFormatter();
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
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists) {
                fileInfo.MoveTo(Path.Combine(fileInfo.DirectoryName ?? "", newName));
                return true;
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
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
        return ((encoding != null) ? encoding : Utf8NoBom).GetString(content);
    }

    public static byte[] StringToByteArr(string content, Encoding encoding = null) {
        return ((encoding != null) ? encoding : Utf8NoBom).GetBytes(content);
    }

    public static string SizeSuffix(long value, int decimalPlaces = 1) {
        if (value < 0) {
            return "-" + SizeSuffix(-value);
        }
        int num = 0;
        decimal num2 = value;
        while (Math.Round(num2, decimalPlaces) >= 1000m) {
            num2 /= 1024m;
            num++;
        }

        var formatter = "{0:n" + decimalPlaces + "} {1}";
        return string.Format(formatter, num2, SizeSuffixes[num]);
    }

    public static bool FileEquals(string file1, string file2) {
        FileInfo fileInfo = new FileInfo(file1);
        FileInfo fileInfo2 = new FileInfo(file2);
        if (!fileInfo.Exists || !fileInfo2.Exists || fileInfo.Length != fileInfo2.Length) {
            return false;
        }
        byte[] array = File.ReadAllBytes(file1);
        byte[] array2 = File.ReadAllBytes(file2);
        int i = 0;
        for (int num = array.Length; i < num; i++) {
            if (array[i] != array2[i]) {
                return false;
            }
        }
        return true;
    }
}
}