using UnityEngine;
using UnityEngine.UI;
using NilanToolkit.UiExtensions;

namespace NilanToolkit.Tweener {
    public static class UnityTweenShortcut {

    #region GameObject

    /// <summary>
    /// fade to endValue in duration
    /// </summary>
    /// <param name="target"></param>
    /// <param name="endValue"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static ITweener DoFade(this GameObject target, float endValue, float duration) {
        return Tween.To(
        () => target.transform.GetOpacity(), 
        (value)=> {
            target.transform.SetOpacity(value);
        }, endValue, duration);
    }

    #endregion

    #region Transform

    /// <summary>
    /// move to somewhere in duration by Transform.position
    /// </summary>
    public static ITweener DoMove(this Transform target, Vector3 endValue, float duration) {
        return Tween.To(() => { return target.position; }, (val) => { target.position = val; },endValue,duration);
    }
    
    /// <summary>
    /// move to somewhere in duration by Transform.localPosition
    /// </summary>
    public static ITweener DoLocalMove(this Transform target, Vector3 endValue, float duration) {
        return Tween.To(() => { return target.localPosition; }, (val) => { target.localPosition = val; },endValue,duration);
    }

    /// <summary>
    /// scale to a size in duration
    /// </summary>
    public static ITweener DoLocalScale(this Transform target, Vector3 endValue, float duration) {
        return Tween.To(() => { return target.localScale; }, (val) => { target.localScale = val; },endValue,duration);
    }

    /// <summary>
    /// rotate a transform in duration
    /// </summary>
    public static ITweener DoRotate(this Transform target, Quaternion endValue, float duration) {
        return Tween.To(() => { return target.rotation; }, (val) => { target.rotation = val; },endValue,duration);
    }

    /// <summary>
    /// rotate a transform in duration by eulerAngle
    /// </summary>
    public static ITweener DoRotate (this Transform target, Vector3 endValue, float duration) {
        return Tween.To(() => { return target.rotation; }, (val) => { target.rotation = val; }, Quaternion.Euler(endValue),duration);
    }

    /// <summary>
    /// transition to a status in duration
    /// </summary>
    public static ITweener DoStatus(this Transform target, TransformStatus endValue, float duration) {
        var t = new BaseTweener<TransformStatus>();
        t.Getter = () => TransformStatus.Create(target);
        t.Setter = val => val.Apply(target);
        t.TweenFunc = TransformStatus.Lerp;
        t.Target = endValue;
        t.Duration = duration;
        return t;
    }

    #endregion

    #region RectTransform

    /// <summary>
    /// move to somewhere in duration by RectTransform.anchoredPosition
    /// </summary>
    public static ITweener DoAnchoredMove(this RectTransform target, Vector2 endValue, float duration) {
        return Tween.To(() => { return target.anchoredPosition;}, val => { target.anchoredPosition = val;},endValue,duration);
    }
    
    /// <summary>
    /// transition RectTransform.sizeDelta in duration
    /// </summary>
    public static ITweener DoSizeDelta(this RectTransform target, Vector2 endValue, float duration) {
        return Tween.To(() => { return target.sizeDelta;}, val => { target.sizeDelta = val;},endValue,duration);
    }

    #endregion

    #region Graphic

    public static ITweener DoColor(this Graphic target, Color endValue, float duration) {
        return Tween.To(() => { return target.color; }, val => target.color = val, endValue, duration);
    }

    #endregion
    
}

}
