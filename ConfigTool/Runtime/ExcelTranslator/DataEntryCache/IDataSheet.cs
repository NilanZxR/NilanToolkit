using System.Collections.Generic;

namespace NilanToolkit.ConfigTool {
    
    public interface IDataSheet {
        DataBlockBase this[string key] { get; set; }
        
    }

    public interface IDataSheet<T> : IEnumerable<KeyValuePair<string, T>> where T : DataBlockBase{
        T this[string key] { get; set; }

        IEnumerable<T> Blocks { get; }

        bool TryGetValue(string key, out T value);

    }
    
}
