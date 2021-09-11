namespace NilanToolkit.Tweener {
    public class BaseTweener<TValue> : ITweener, IRelativeableTweener {
        
        public bool IsActive { get; set; }
        public float StartTime { get; set; }
        public float Duration { get; set; }
        public float CurrentTime { get; set; }

        public event TweenCallback OnComplete;
    
        public TweenGetter<TValue> Getter { get; set; }

        public TweenSetter<TValue> Setter { get; set; }

        public TweenFunc<TValue> TweenFunc { get; set; }
    
        public TValue Target { get; set; }

        internal TValue _beginValue, _endValue;
    
        public bool IsRelative { get; set; }

        public virtual void Setup() {
            _beginValue = Getter();
            _endValue = Target;
        }

        internal TValue CalculateValue() {
            if (Duration < 0.01f) {
                return _endValue;
            }
            var t = (CurrentTime - StartTime) / Duration;
            var val = TweenFunc(_beginValue, _endValue, t);
            return val;
        }
    
        public void Update() {
            var val = CalculateValue();
            Setter(val);
        }

        public void Complete() {
            FastForward();
            OnComplete?.Invoke();
        }

        public void FastForward() {
            CurrentTime = Duration;
            Update();
        }
        
    }
}
