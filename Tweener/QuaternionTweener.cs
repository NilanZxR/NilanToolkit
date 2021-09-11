using UnityEngine;

namespace NilanToolkit.Tweener {
    public class QuaternionTweener : BaseTweener<Quaternion> {
        public QuaternionTweener(TweenGetter<Quaternion> getter, TweenSetter<Quaternion> setter, Quaternion endValue,
                                 float duration) {
            Getter = getter;
            Setter = setter;
            TweenFunc = QuaternionLerp;
            Target = endValue;
            StartTime = 0;
            Duration = duration;
        }

        private Quaternion QuaternionLerp(Quaternion a, Quaternion b, float t) {
            if (Quaternion.Dot(a, b) < Quaternion.kEpsilon) {
                return Quaternion.identity;
            }

            return Quaternion.Lerp(a, b, t);
        }
    }

}
