using UnityEngine;

namespace NilanToolkit.Tweener {
    public class ColorTweener : BaseTweener<Color> {
        public ColorTweener(TweenGetter<Color> getter, TweenSetter<Color> setter, Color endValue, float length) {
            Getter = getter;
            Setter = setter;
            TweenFunc = Color.Lerp;
            Target = endValue;
            StartTime = 0;
            Duration = length;
        }

        public override void Setup() {
            base.Setup();
            if (IsRelative) {
                _endValue = _beginValue + Target;
            }
        }
    }

}
