using System.Collections.Generic;
using NilanToolkit.Ticking;
using UnityEngine;

namespace NilanToolkit.Tweener {
    /// <summary>
    /// A tool to tween the value and do some motions, just like DoTween
    /// </summary>
    public static class Tween {

        internal static Dictionary<ITweener, Ticker> BindedTweeners = new Dictionary<ITweener, Ticker>();

        /// <summary>
        /// create a tweener that do nothing, just wait a few of time
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static EmptyTweener Delay(float duration)
        {
            return new EmptyTweener(duration);
        }

        /// <summary>
        /// all contained tweener will start at the same time
        /// </summary>
        /// <returns></returns>
        public static TweenParallel Parallel ()
        {
            return new TweenParallel();
        }

        public static TweenParallel Parallel(params ITweener[] tweeners)
        {
            return new TweenParallel(tweeners);
        }

        /// <summary>
        /// all contained tweener will start one by one
        /// </summary>
        /// <returns></returns>
        public static TweenSequence Sequence ()
        {
            return new TweenSequence();
        }

        public static TweenSequence Sequence(params ITweener[] tweeners)
        {
            return new TweenSequence(tweeners);
        }

        public static IntTweener To(TweenGetter<int> getter, TweenSetter<int> setter, int endValue, float duration)
        {
            return new IntTweener(getter, setter, endValue, duration);
        }

        public static FloatTweener To(TweenGetter<float> getter, TweenSetter<float> setter, float endValue, float duration)
        {
            return new FloatTweener(getter, setter, endValue, duration);
        }

        public static DoubleTweener To(TweenGetter<double> getter, TweenSetter<double> setter, double endValue, float duration)
        {
            return new DoubleTweener(getter, setter, endValue, duration);
        }

        public static Vector2Tweener To(TweenGetter<Vector2> getter, TweenSetter<Vector2> setter, Vector2 endValue, float duration)
        {
            return new Vector2Tweener(getter, setter, endValue, duration);
        }

        public static Vector3Tweener To(TweenGetter<Vector3> getter, TweenSetter<Vector3> setter, Vector3 endValue, float duration)
        {
            return new Vector3Tweener(getter, setter, endValue, duration);
        }

        public static Vector4Tweener To(TweenGetter<Vector4> getter, TweenSetter<Vector4> setter, Vector4 endValue, float duration)
        {
            return new Vector4Tweener(getter, setter, endValue, duration);
        }

        public static QuaternionTweener To(TweenGetter<Quaternion> getter, TweenSetter<Quaternion> setter, Quaternion endValue, float duration)
        {
            return new QuaternionTweener(getter, setter, endValue, duration);
        }

        public static ColorTweener To(TweenGetter<Color> getter, TweenSetter<Color> setter, Color endValue, float duration)
        {
            return new ColorTweener(getter, setter, endValue, duration);
        }

        internal static Ticker BindTweener(ITweener tweener)
        {
            if (BindedTweeners.TryGetValue(tweener, out var ticker)) {
                return ticker;
            }
            else {
                ticker = Ticker.Create();
                BindedTweeners.Add(tweener, ticker);
                return ticker;
            }
        }

        internal static void StopTweenerTicking(ITweener tweener)
        {
            if (BindedTweeners.TryGetValue(tweener, out var ticker)) {
                ticker.IsFinish = true;
            }

            BindedTweeners.Remove(tweener);
        }

        internal static void Recovery(ITweener tweener)
        {
            //TODO make tweener reuseable
        }
    }
}
