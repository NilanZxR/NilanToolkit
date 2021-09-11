namespace NilanToolkit.Tweener {
    
    /// <summary>
    /// get the value from owner
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public delegate TValue TweenGetter<TValue>();

    /// <summary>
    /// set the value to owner
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="TValue"></typeparam>
    public delegate void TweenSetter<TValue>(TValue value);

    /// <summary>
    /// return a value between a and b by t
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <typeparam name="TValue"></typeparam>
    public delegate TValue TweenFunc<TValue>(TValue a, TValue b, float t);

    public delegate void TweenCallback();

}
