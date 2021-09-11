using UnityEngine;
using System.Collections;
using NilanToolkit.Event;

namespace NilanToolkit {

    public class Timer : MonoBehaviour {

        public enum TimeUpdateType {
            Update,
            FixedUpdate,
            Custom
        }

        private float _totalTime;

        private bool _ticking;

        public bool DestoryWhenTimeup { get; set; }

        public TimeUpdateType UpdateType { get; set; }

        public float Duration { get; set; }

        public EventHandler onTimeup { get; private set; }

        public void StartTicking(bool reset = true) {
            if (reset) Reset();
            _ticking = true;
        }

        public void StopTicking() {
            _ticking = false;
        }

        public void Reset() {
            _totalTime = 0f;
        }

        public void Tick(float deltaTime) {
            if (!_ticking) return;

            _totalTime += deltaTime;
            if (Duration > 0f && _totalTime >= Duration) {
                _ticking = false;
                onTimeup.Invoke();

                if (DestoryWhenTimeup) {
                    Destroy(gameObject);
                }
            }
        }

        public virtual void OnTick() { }

        private void Awake() {
            onTimeup = new EventHandler();
        }

        private void Update() {
            if (UpdateType != TimeUpdateType.Update) return;
            Tick(Time.deltaTime);
        }

        private void FixedUpdate() {
            if (UpdateType != TimeUpdateType.FixedUpdate) return;
            Tick(Time.fixedDeltaTime);
        }

    }

}


