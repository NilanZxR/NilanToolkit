using System.IO;
using System.Text;
using UnityEngine;

namespace NilanToolkit.ConfigTool
{
    public static class PathUtils
    {

        public static string ToUnityRelativePath(string absolutePath)
        {
            var sb = new StringBuilder();
            absolutePath = absolutePath.Replace("\\", "/");
            int indexOf = absolutePath.IndexOf(Application.dataPath + "/");
            if (indexOf == 0)
            {
                return "{0}/" + absolutePath.Substring(Application.dataPath.Length + 1);
            }
            
            var destdir = Directory.CreateDirectory(absolutePath);
            var assetdir = Directory.CreateDirectory(Application.dataPath);

            sb.Clear();
            sb.Append("{0}/");

            for (var parent = assetdir.Parent; parent != null; parent = parent.Parent)
            {
                sb.Append("../");
                for (var dest = destdir.Parent; dest != null; dest = dest.Parent)
                {
                    if (dest.FullName == parent.FullName)
                    {
                        string parentPath = parent.FullName.Replace("\\", "/");
                        string path = absolutePath.Substring(parentPath.Length + 1);
                        sb.Append(path);
                        return sb.ToString();
                    }
                }
            }

            return absolutePath;
        }

        public static string ToAbsolutePath(string relativePath)
        {
            if (relativePath.StartsWith("{0}/"))
                return string.Format(relativePath, Application.dataPath);
            return relativePath;
        }

    }
}
