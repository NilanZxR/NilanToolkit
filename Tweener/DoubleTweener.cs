using UnityEngine;

namespace NilanToolkit.Tweener {
    public class DoubleTweener : BaseTweener<double> {
        public DoubleTweener(TweenGetter<double> getter, TweenSetter<double> setter, double endValue, float duration) {
            Getter = getter;
            Setter = setter;
            TweenFunc = DoubleLerp;
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

        public static double DoubleLerp(double a,double b,float t) {
            return a + (b - a) * Mathf.Clamp01(t);
        }

    }
}
