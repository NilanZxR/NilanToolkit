using System.Collections.Generic;

namespace NilanToolkit.ConfigTool {
    public class DataSheet {
        public readonly string sheetName;
        public readonly int rows;
        public readonly int columns;

        public Dictionary<string, DataEntryBase> Sheet { get; }

        public DataEntryBase this[string id] {
            get {
                Sheet.TryGetValue(id, out var result);
                return result;
            }
            set => Sheet[id] = value;
        }

        public T GetEntry<T>(string id) where T : DataEntryBase {
            Sheet.TryGetValue(id, out var data);
            return data as T;
        }

        public DataSheet(string sheetName, int rows, int columns) {
            this.sheetName = sheetName;
            this.rows = rows;
            this.columns = columns;
            Sheet = new Dictionary<string, DataEntryBase>();
        }
    }
}
