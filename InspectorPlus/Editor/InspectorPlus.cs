using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;

namespace NilanToolkit.InspectorPlus {
    public class InspectorPlus {
        public static VisualElement DrawMethod(object target, MethodInfo info, string label = null) {
            var container = new VisualElement();
            if (string.IsNullOrEmpty(label)) {
                label = info.Name;
            }
            var tit = new Label(label);
            container.Add(tit);

            var lst = new List<VisualElement>();
            var parameterInfos = info.GetParameters();
            foreach (var parameterInfo in parameterInfos) {
                var type = parameterInfo.ParameterType;
                var paramVe = CreateVeByType(type);
                lst.Add(paramVe);
                container.Add(paramVe);
            }

            Action action = () => {
                var param = lst.Select(item => {
                    var t = item.GetType();
                    var property = t.GetProperty("value");
                    return property.GetValue(item);
                });
                info.Invoke(target, param.ToArray());
            };
            var button = new Button(action);
            button.text = "Invoke";
            container.Add(button);
            return container;
        }

        public static VisualElement CreateVeByType(Type type, string label = null) {
            if (string.IsNullOrEmpty(label)) {
                label = type.Name;
            }
            if (type == typeof(string)) return new TextField(label);
            if (type == typeof(int)) return new IntegerField(label);
            if (type == typeof(float)) return new FloatField(label);
            if (type == typeof(double)) return new DoubleField(label);
            if (type == typeof(long)) return new LongField(label);
            if (type == typeof(Vector2)) return new Vector2Field(label);
            if (type == typeof(Vector3)) return new Vector3Field(label);
            if (type == typeof(Vector4)) return new Vector4Field(label);
            if (type == typeof(Color)) return new ColorField(label);
            if (type == typeof(Enum)) return new EnumField(label);
            return new ObjectField(label);
        }
    }
}
