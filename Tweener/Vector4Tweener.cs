using UnityEngine;

namespace NilanToolkit.Tweener {
    public class Vector4Tweener : BaseTweener<Vector4> {
        public Vector4Tweener(TweenGetter<Vector4> getter, TweenSetter<Vector4> setter, Vector4 endValue, float duration) {
            Getter = getter;
            Setter = setter;
            TweenFunc = Vector4.Lerp;
            Target = endValue;
            StartTime = 0;
            Duration = duration;
        }

        public override void Setup() {
            base.Setup();
            if (IsRelative) {
                _endValue = _beginValue + Target;
            }
        }
    }

}
