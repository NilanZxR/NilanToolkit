using System;
using System.Collections.Generic;

namespace NilanToolkit.DataModel {
public class ObservedObject {
    internal object value;
    public bool Dirty { get; set; }
    
    public void SetDirty() {
        Dirty = true;
    }

    internal virtual void WriteValue(object value) {
        this.value = value;
        SetDirty();
    }

    internal virtual ObservedObject Search(ref Stack<string> stack) {
        if (stack.Count <= 0) return this;
        if (value is ObservedObject val) {
            return val.Search(ref stack);
        }
        else {
            throw new Exception("invalid path");
        }
    }

}
}