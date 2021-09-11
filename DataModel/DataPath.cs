using System.Collections.Generic;

namespace NilanToolkit.DataModel {
public struct DataPath {

    public string Path { get; internal set; }
    
    public int Depth { get; internal set; }
    
    public string[] SubPaths { get; internal set; }
    
    public bool IsEmpty { get; private set; }

    public static DataPath Create(string path) {
        var strings = path.Split('/');
        var stack = new Stack<string>();
        for (var index = 0; index < strings.Length; index++) {
            var s = strings[index];
            switch (s) {
                case "":
                case ".": {
                    break;
                }
                case "..": {
                    if (stack.Count > 0) {
                        stack.Pop();
                    }
                    break;
                }
                default: {
                    stack.Push(s);
                    break;
                }
            }
        }

        var subPaths = stack.ToArray();
        var depth = stack.Count;
        var p = string.Join("/", subPaths);
        
        return new DataPath() {
            Path = p,
            Depth = depth,
            SubPaths = subPaths
        };
    }
    

    public static DataPath Combine(DataPath a, DataPath b) {
        var path = a.Path + "/" + b.Path;
        return Create(path);
    }

    public static DataPath Combine(DataPath a, string b) {
        var path = a.Path + "/" + b;
        return Create(path);
    }

    public static DataPath Combine(string a, DataPath b) {
        var path = a + "/" + b.Path;
        return Create(path);
    }

    public static DataPath operator +(DataPath a, DataPath b) {
        return Combine(a, b);
    }

    public static DataPath Empty() {
        return new DataPath(){IsEmpty =  true};
    }
    
}
}