using System;
using System.Collections.Generic;
using System.Text;

namespace NilanToolkit.DataBinding {
public struct DataPath : IEquatable<DataPath> {

    public string Path { get; internal set; }

    public bool IsNullOrEmpty => string.IsNullOrEmpty(Path);

    public DataPath(string path) {
        Path = path;
    }

    public string[] GetHierarchy() {
        if (IsNullOrEmpty) return Array.Empty<string>();
        return Path.Split('/');
    }

    public DataPath GetParentPath(int absoluteDepth) {
        var depth = 0;
        for (var i = 0; i < Path.Length; i++) {
            var c = Path[i];
            if (c == '/') depth++;
            if (depth > absoluteDepth) {
                return new DataPath(Path.Substring(0, i));
            }
        }

        throw new Exception("invalid depth");
    }

    public DataPath GetParentPathRelative(int relativeDepth) {
        var rDepth = 0;
        for (var i = Path.Length - 1; i >= 0; i--) {
            var c = Path[i];
            if (c == '/') rDepth++;
            if (rDepth > relativeDepth) {
                return new DataPath(Path.Substring(0, i));
            }
        }

        throw new Exception("invalid depth");
    }

    public bool IsParentOf(string otherPath) {
        return IsParentPath(Path, otherPath);
    }

    public bool IsSubPathOf(string otherPath) {
        return IsParentPath(otherPath, Path);
    }
    

    public static DataPath Combine(DataPath a, DataPath b) {
        var path = a.Path + "/" + b.Path;
        return new DataPath(path);
    }

    public static DataPath operator +(DataPath a, DataPath b) {
        return Combine(a, b);
    }

    public static bool operator ==(DataPath a, DataPath b) {
        return a.Equals(b);
    }

    public static bool operator !=(DataPath a, DataPath b) {
        return !a.Equals(b);
    }
    
    public static implicit operator string(DataPath dataPath) {
        return dataPath.ToString();
    }

    public static implicit operator DataPath(string path) {
        return new DataPath(path);
    }

    public static bool IsParentPath(string parent, string sub) {
        return parent.StartsWith(sub);
    }

    public override string ToString() {
        return Path;
    }

    public bool Equals(DataPath other) {
        return Path == other.Path;
    }

    public override bool Equals(object obj) {
        return obj is DataPath other && Equals(other);
    }

    public override int GetHashCode() {
        return (Path != null ? Path.GetHashCode() : 0);
    }
}
}