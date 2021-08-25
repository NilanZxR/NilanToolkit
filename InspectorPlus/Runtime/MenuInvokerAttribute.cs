using System;

namespace NilanToolkit.InspectorPlus {

    [AttributeUsage(AttributeTargets.Method)]
    public class MenuInvokerAttribute : Attribute {
        public string Title;

        public MenuInvokerAttribute() { }

        public MenuInvokerAttribute(string title) {
            Title = title;
        }
    }
}
