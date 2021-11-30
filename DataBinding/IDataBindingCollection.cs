using System.Collections.Generic;

namespace NilanToolkit.DataBinding {
    public interface IDataBindingCollection : IEnumerable<BindingObject> {
        BindingObject GetItem(string key);

    }
}
