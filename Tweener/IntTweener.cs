using UnityEngine;

namespace NilanToolkit.Tweener {
    public class IntTweener : BaseTweener<int> {
        public IntTweener(TweenGetter<int> getter, TweenSetter<int> setter, int endValue, float duration) {
            Getter = getter;
            Setter = setter;
            TweenFunc = IntLerp;
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

        public static int IntLerp(int a, int b, float t) {
            return Mathf.FloorToInt(Mathf.Lerp(a, b, t));
        }
    }

}
