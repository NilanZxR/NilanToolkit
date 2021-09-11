using System;
using System.Collections;
using NilanToolkit.Ticking;
using UnityEngine;

namespace NilanToolkit.Tweener {
    public static class TweenShortcut {

        /// <summary>
        /// make a tweener parallel to other tweener, and return a tweener contained them, two tweeners will start at the same time
        /// if this tween is a part of sequence, after the longest one finish, the next tweener will begin 
        /// </summary>
        /// <param name="tweener"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static ITweener Join(this ITweener tweener, ITweener other)
        {
            var parallel = tweener as TweenParallel;
            if (parallel == null) {
                parallel = Tween.Parallel(tweener);
            }
            parallel.Add(other);
            return parallel;
        }

        /// <summary>
        /// connect another tweener to end of tweener
        /// </summary>
        /// <param name="tweener"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static ITweener Append(this ITweener tweener, ITweener other)
        {
            var seq = tweener as TweenSequence;
            if (seq == null) {
                seq = Tween.Sequence(tweener);
            }

            seq.Add(other);
            return seq;
        }

        /// <summary>
        /// set interval to end
        /// </summary>
        /// <param name="tweener"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static ITweener AppendInterval(this ITweener tweener, float duration)
        {
            return tweener.Append(Tween.Delay(duration));
            ;
        }

        /// <summary>
        /// Play the motion of tweener
        /// </summary>
        /// <param name="tweener"></param>
        public static ITweener Start(this ITweener tweener)
        {
            tweener.IsActive = true;
            tweener.Setup();

            if (tweener.Duration < 0.01f) {
                tweener.FastForward();
                tweener.Complete();
                return tweener;
            }

            var ticker = Tween.BindTweener(tweener);
            ticker.OnTick = delta => {
                if (!tweener.IsActive) {
                    Tween.StopTweenerTicking(tweener);
                    tweener.Complete();
                    return;
                }
                if (tweener.CurrentTime > tweener.Duration) {
                    tweener.IsActive = false;
                }
                tweener.Update();
                tweener.CurrentTime += delta;
            };
            TickCore.Instance.RegisterTicker(ticker);

            return tweener;
        }

        /// <summary>
        /// create a enumerator by tweener, step added by UnityEngine.Time.deltaTime
        /// </summary>
        /// <param name="tweener"></param>
        /// <returns></returns>
        public static IEnumerator GetEnumerator(this ITweener tweener)
        {
            while (tweener.CurrentTime < tweener.Duration) {
                tweener.Update();
                tweener.CurrentTime += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// create a custom step additional enumerator
        /// </summary>
        /// <param name="tweener"></param>
        /// <param name="stepAdd"></param>
        /// <returns></returns>
        public static IEnumerator GetEnumerator(this ITweener tweener, Func<float> stepAdd)
        {
            while (tweener.CurrentTime < tweener.Duration) {
                tweener.Update();
                tweener.CurrentTime += stepAdd();
                yield return null;
            }
        }

        /// <summary>
        /// set a tweener motion mode to relative, not all tweener is relativeable
        /// </summary>
        /// <param name="tweener"></param>
        /// <param name="relative"></param>
        /// <returns></returns>
        public static ITweener SetRelative(this ITweener tweener, bool relative = true)
        {
            var relativeableTweener = tweener as IRelativeableTweener;
            if (relativeableTweener != null) {
                relativeableTweener.IsRelative = relative;
            }

            return tweener;
        }

        /// <summary>
        /// set a callback when tweener finished
        /// </summary>
        /// <param name="tweener"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static ITweener SetCompleteListener(this ITweener tweener, TweenCallback callback)
        {
            tweener.OnComplete += callback;
            return tweener;
        }

        public static ITweener Stop(this ITweener tweener)
        {
            Tween.StopTweenerTicking(tweener);
            return tweener;
        }

        public static ITweener Kill(this ITweener tweener)
        {
            tweener.IsActive = false;
            if (Tween.BindedTweeners.ContainsKey(tweener)) {
                Tween.StopTweenerTicking(tweener);
            }
            Tween.Recovery(tweener);
            return tweener;
        }

        public static ITweener FastForwardAndFinish(this ITweener tweener)
        {
            tweener.FastForward();
            return tweener;
        }

        public static ITweener DestroyWith(this ITweener tweener, UnityEngine.Object obj)
        {
            tweener.OnComplete += () => UnityEngine.Object.Destroy(obj);
            return tweener;
        }

    }

}
