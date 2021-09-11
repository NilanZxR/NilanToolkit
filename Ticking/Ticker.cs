namespace NilanToolkit.Ticking {
    
    public delegate void TickStartDel();

    public delegate void TickEndDel();
        
    public delegate void TickDel(float deltaTime);
    
    public class Ticker {
        private Ticker() { }

        public TickStartDel OnTickStart;
        public TickEndDel OnTickEnd;
        public TickDel OnTick;
    
        public bool Pause;

        public bool Pauseable = true;

        public bool Active;

        public bool IsFinish;

        public static Ticker Create() {
            return new Ticker();
        }

        public void CallTickStart() {
            Active = true;
            OnTickStart?.Invoke();
        }

        public void CallTickEnd() {
            Active = false;
            OnTickEnd?.Invoke();
        }

        public void Tick(float deltaTime) {
            if (Pause && Pauseable) return;
            OnTick?.Invoke(deltaTime);
        }

    }
}
