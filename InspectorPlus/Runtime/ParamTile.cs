using System;

namespace NilanToolkit.InspectorPlus {
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParamTitleAttribute : Attribute {
        public string Title;

        public ParamTitleAttribute(string title) {
            Title = title;
        }
    }
}
