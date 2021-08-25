#if INSPECTOR_PLUS

using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace NilanToolkit.InspectorPlus {
    [UnityEditor.CustomEditor(typeof(UnityEngine.Object), true)]
    public class OverrideDefaultInspector : UnityEditor.Editor {

        public override VisualElement CreateInspectorGUI() {
            IMGUIContainer defaultInspector = new IMGUIContainer(() => DrawDefaultInspector());

            var visualElement = new VisualElement();
            visualElement.Add(defaultInspector);

            var allNeedDrawMethod = ReflectionUtil.GetAllMethodByAttribute<MenuInvokerAttribute>(target.GetType());
            foreach (var methodInfo in allNeedDrawMethod) {
                var methodVe = InspectorPlus.DrawMethod(target, methodInfo);
                visualElement.Add(methodVe);
            }

            return visualElement;
        }


    }
}

#endif
