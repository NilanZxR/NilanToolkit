namespace NilanToolkit.Tweener {
    public class EmptyTweener : ITweener {
        public bool IsActive { get; set; }
        public float StartTime { get; }
        public float Duration { get; set; }
        public float CurrentTime { get; set; }

        public event TweenCallback OnComplete;

        public EmptyTweener(float duration) {
            Duration = duration;
        }

        public void Setup() { }
    
        public void Update() { }
    
        public void Complete() {
            OnComplete?.Invoke();
        }

        public void FastForward() {
            CurrentTime = Duration;
        }

    }
}
