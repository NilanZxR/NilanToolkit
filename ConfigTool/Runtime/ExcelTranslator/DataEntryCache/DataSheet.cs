using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NilanToolkit.ConfigTool {

    public class DataSheet<T> :
    IDataSheet,
    IDataSheet<T>
    where T : DataBlockBase 
    {
        public readonly string sheetName;
        public readonly int rows;
        public readonly int columns;

        public Dictionary<string, T> Dataset { get; }

        public T this[string id] {
            get {
                Dataset.TryGetValue(id, out var result);
                return result;
            }
            set => Dataset[id] = value;
        }

        public IEnumerable<T> Blocks => Dataset.Values;
        
        public bool TryGetValue(string key, out T value) {
            return Dataset.TryGetValue(key, out value);
        }

        public bool TryGetValue(string key, out DataBlockBase value) {
            var isExist = Dataset.TryGetValue(key, out var val);
            value = val;
            return isExist;
        }

        public T Get(string id) {
            Dataset.TryGetValue(id, out var data);
            return data;
        }

        public DataSheet(string sheetName, int rows, int columns) {
            this.sheetName = sheetName;
            this.rows = rows;
            this.columns = columns;
            Dataset = new Dictionary<string, T>();
        }
        DataBlockBase IDataSheet.this[string key] {
            get => Get(key);
            set => Dataset[key] = (T) value;
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator() {
            return Dataset.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

}
