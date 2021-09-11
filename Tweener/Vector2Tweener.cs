using UnityEngine;

namespace NilanToolkit.Tweener {
    public class Vector2Tweener : BaseTweener<Vector2> {
        public Vector2Tweener(TweenGetter<Vector2> getter, TweenSetter<Vector2> setter, Vector2 endValue, float duration) {
            Getter = getter;
            Setter = setter;
            TweenFunc = Vector2.Lerp;
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
