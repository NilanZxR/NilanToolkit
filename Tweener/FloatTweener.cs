using UnityEngine;

namespace NilanToolkit.Tweener {
    public class FloatTweener : BaseTweener<float> {
        public FloatTweener(TweenGetter<float> getter, TweenSetter<float> setter, float endValue, float duration) {
            Getter = getter;
            Setter = setter;
            TweenFunc = Mathf.Lerp;
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
