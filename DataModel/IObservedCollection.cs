using System.Collections.Generic;

namespace NilanToolkit.DataModel {
    public interface IObservedCollection : IEnumerable<ObservedObject> {
        ObservedObject GetItem(string key);
    }
}
