using System.IO;
using System.Text;
using UnityEngine;

namespace NilanToolkit.ConfigTool
{
    public static class PathUtils
    {
        private static StringBuilder __StringBuilder = new StringBuilder();

        public static string GetAssetDatabasePath(string fullPath)
        {
            int index = fullPath.IndexOf("Assets");
            return fullPath.Substring(index);
        }

        public static string GetRelativePath(string fullPath, string root)
        {
            fullPath = fullPath.Replace("\\", "/");
            int index = fullPath.IndexOf(root) + root.Length;
            if (fullPath.Length > index + 1)
            {
                index += 1;
            }
            return fullPath.Substring(index);
        }

        public static string AbsolutePathToUnityRelativePath(string absolutePath)
        {
            try
            {
                absolutePath = absolutePath.Replace("\\", "/");
                int indexOf = absolutePath.IndexOf(Application.dataPath + "/");
                if (indexOf == 0)
                {
                    return "{0}/" + absolutePath.Substring(Application.dataPath.Length + 1);
                }
                else
                {
                    var destdir = Directory.CreateDirectory(absolutePath);
                    var assetdir = Directory.CreateDirectory(Application.dataPath);

                    __StringBuilder.Clear();
                    __StringBuilder.Append("{0}/");

                    for (var parent = assetdir.Parent; parent != null; parent = parent.Parent)
                    {
                        __StringBuilder.Append("../");
                        for (var dest = destdir.Parent; dest != null; dest = dest.Parent)
                        {
                            if (dest.FullName == parent.FullName)
                            {
                                string parentPath = parent.FullName.Replace("\\", "/");
                                string path = absolutePath.Substring(parentPath.Length + 1);
                                __StringBuilder.Append(path);
                                return __StringBuilder.ToString();
                            }
                        }
                    }

                    return absolutePath;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public static string UnityRelativePathToAbsolutePath(string relativePath)
        {
            if (relativePath.StartsWith("{0}/"))
                return string.Format(relativePath, Application.dataPath);
            return relativePath;
        }

    }
}
