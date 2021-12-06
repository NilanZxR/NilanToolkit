using System;
using System.Collections.Generic;

namespace NilanToolkit.DataBinding {
public class BindingObject {
    internal object value;
    public bool Dirty { get; set; }
    
    public void SetDirty() {
        Dirty = true;
    }

    internal virtual void WriteValue(object value) {
        this.value = value;
        SetDirty();
    }

    internal virtual BindingObject Search(ref Stack<string> stack) {
        if (stack.Count <= 0) return this;
        if (value is BindingObject val) {
            return val.Search(ref stack);
        }
        else {
            throw new Exception("invalid path");
        }
    }

}
}