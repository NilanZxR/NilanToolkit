using UnityEngine;

namespace NilanToolkit.Tweener {
    public class Vector3Tweener : BaseTweener<Vector3> {
        public Vector3Tweener(TweenGetter<Vector3> getter, TweenSetter<Vector3> setter, Vector3 endValue, float duration) {
            Getter = getter;
            Setter = setter;
            TweenFunc = Vector3.Lerp;
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
