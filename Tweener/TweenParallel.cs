using System.Collections.Generic;
using System.Linq;

namespace NilanToolkit.Tweener {
    public class TweenParallel : ITweenCollection {

    public bool IsActive { get; set; }

    public int Count => Tweeners.Count;

    public ITweener this[int index] => Tweeners[index];


    public float StartTime {
        get { return 0f; }
    }

    public float Duration {
        get { return CalculateDuration(); }
    }

    public float CurrentTime { get; set; }

    public event TweenCallback OnComplete;

    internal List<ITweener> Tweeners;
    
    public void Add(ITweener tweener) {
        Tweeners.Add(tweener);
    }

    public void Setup() {
        for (var i = 0; i < Tweeners.Count; i++) {
            var tweener = Tweeners[i];
            tweener.Setup();
        }
    }

    public void Update() {
        var curr = CurrentTime;
        for (var i = 0; i < Tweeners.Count; i++) {
            var tweener = Tweeners[i];
            if (tweener == null) continue;
            if (curr <= tweener.Duration) {
                tweener.CurrentTime = curr;
                tweener.Update();
            }
            else {
                if (tweener.CurrentTime < tweener.Duration) {
                    tweener.CurrentTime = tweener.Duration;
                    tweener.Update();
                    tweener.Complete();
                }
            }
        }
    }

    public float CalculateDuration() {
        var ret = 0f;
        for (var index = 0; index < Tweeners.Count; index++) {
            var tweener = Tweeners[index];
            if (ret < tweener.Duration) ret = tweener.Duration;
        }

        return ret;
    }

    public void Complete() {
        OnComplete?.Invoke();
    }

    public void FastForward() {
        CurrentTime = Duration;
        for (var index = 0; index < Tweeners.Count; index++) {
            var tweener = Tweeners[index];
            tweener.FastForward();
        }
    }
    internal TweenParallel() {
        Tweeners = new List<ITweener>();
    }

    internal TweenParallel(IEnumerable<ITweener> initItems) {
        if (initItems == null) {
            Tweeners = new List<ITweener>();
        }
        else {
            Tweeners = initItems.ToList();
        }
    }
}

}
