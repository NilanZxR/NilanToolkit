using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NilanToolkit.Tweener {
    public class TweenSequence : ITweenCollection {

        public int Count => Tweeners.Count;

        public ITweener this[int index] => Tweeners[index];

        public bool IsActive { get; set; }
        public float StartTime => 0f;

        public float Duration => CalculateDuration();

        public float CurrentTime { get; set; }

        public event TweenCallback OnComplete;


        internal List<ITweener> Tweeners;

        private ITweener _lastTweenItem;

        public void Add(ITweener tweener)
        {
            Tweeners.Add(tweener);
        }

        public void Insert(ITweener tweener, int index)
        {
            index = Mathf.Clamp(index, 0, Tweeners.Count);
            Tweeners.Insert(index, tweener);
        }

        public void Setup ()
        {
            if (Tweeners.Count == 0) return;
            Tweeners[0].Setup();
        }

        public void Update ()
        {
            var curr = CurrentTime;
            for (var index = 0; index < Tweeners.Count; index++) {
                var tweener = Tweeners[index];
                if (tweener == null) continue;

                if (curr <= tweener.Duration) {
                    if (_lastTweenItem != tweener) {
                        _lastTweenItem?.Complete();
                        tweener.Setup();
                        _lastTweenItem = tweener;
                    }
                    tweener.CurrentTime = curr;
                    tweener.Update();
                    return;
                }
                else {
                    if (index == Tweeners.Count - 1) {
                        tweener.CurrentTime = tweener.Duration;
                        tweener.Update();
                        return;
                    }
                    else {
                        curr -= tweener.Duration;
                    }
                }
            }
        }

        public void Complete ()
        {
            OnComplete?.Invoke();
        }

        internal float CalculateDuration ()
        {
            var ret = StartTime;
            for (var index = 0; index < Tweeners.Count; index++) {
                var tweener = Tweeners[index];
                ret += tweener.Duration;
            }

            return ret;
        }

        public void FastForward ()
        {
            var dur = 0f;
            for (var index = 0; index < Tweeners.Count; index++) {
                var tweener = Tweeners[index];
                if (tweener == null) continue;
                dur += tweener.Duration;
                if (CurrentTime > dur) continue;
                tweener.FastForward();
            }

            CurrentTime = Duration;
        }

        internal TweenSequence ()
        {
            Tweeners = new List<ITweener>();
        }

        internal TweenSequence(IEnumerable<ITweener> initItems)
        {
            if (initItems == null) {
                Tweeners = new List<ITweener>();
            }
            else {
                Tweeners = initItems.ToList();
            }
        }
    }

}
