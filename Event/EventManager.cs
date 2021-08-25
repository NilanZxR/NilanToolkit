using System;
using System.Collections.Generic;
public class EventManager {

    private static readonly Dictionary<object, Dictionary<string, Delegate>> s_EventTable = new Dictionary<object, Dictionary<string, Delegate>>();

    private static readonly Dictionary<string, Delegate> s_GlobalEventTable = new Dictionary<string, Delegate>();

    public static void ClearAll() {
        s_EventTable.Clear();
        s_GlobalEventTable.Clear();
    }

    public static void RegisterEvent(string eventName, Action handler) {
        RegisterEvent(eventName, (Delegate) handler);
    }

    public static void RegisterEvent(object obj, string eventName, Action handler) {
        RegisterEvent(obj, eventName, (Delegate) handler);
    }

    public static void RegisterEvent<T>(string eventName, Action<T> handler) {
        RegisterEvent(eventName, (Delegate) handler);
    }

    public static void RegisterEvent<T>(object obj, string eventName, Action<T> handler) {
        RegisterEvent(obj, eventName, (Delegate) handler);
    }

    public static void RegisterEvent<T, U>(string eventName, Action<T, U> handler) {
        RegisterEvent(eventName, (Delegate) handler);
    }

    public static void RegisterEvent<T, U>(object obj, string eventName, Action<T, U> handler) {
        RegisterEvent(obj, eventName, (Delegate) handler);
    }

    public static void RegisterEvent<T, U, V>(string eventName, Action<T, U, V> handler) {
        RegisterEvent(eventName, (Delegate) handler);
    }

    public static void RegisterEvent<T, U, V>(object obj, string eventName, Action<T, U, V> handler) {
        RegisterEvent(obj, eventName, (Delegate) handler);
    }

    public static void RegisterEvent<T, U, V, W>(string eventName, Action<T, U, V, W> handler) {
        RegisterEvent(eventName, (Delegate) handler);
    }

    public static void RegisterEvent<T, U, V, W>(object obj, string eventName, Action<T, U, V, W> handler) {
        RegisterEvent(obj, eventName, (Delegate) handler);
    }

    public static void ExecuteEvent(string eventName) {
        var cb = GetDelegate(eventName) as Action;
        if (cb != null) cb.Invoke();
    }

    public static void ExecuteEvent(object obj, string eventName) {
        Action cb = GetDelegate(obj, eventName) as Action;
        if (cb != null) cb.Invoke();
    }

    public static void ExecuteEvent<T>(string eventName, T arg1) {
        var cb = GetDelegate(eventName) as Action<T>;
        if (cb != null) cb.Invoke(arg1);
    }

    public static void ExecuteEvent<T>(object obj, string eventName, T arg1) {
        var cb = GetDelegate(obj, eventName) as Action<T>;
        if (cb != null) cb.Invoke(arg1);
    }

    public static void ExecuteEvent<T, U>(string eventName, T arg1, U arg2) {
        var cb = GetDelegate(eventName) as Action<T, U>;
        if (cb != null) cb.Invoke(arg1, arg2);
    }

    public static void ExecuteEvent<T, U>(object obj, string eventName, T arg1, U arg2) {
        var cb = GetDelegate(obj, eventName) as Action<T, U>;
        if (cb != null) cb.Invoke(arg1, arg2);
    }

    public static void ExecuteEvent<T, U, V>(string eventName, T arg1, U arg2, V arg3) {
        var cb = GetDelegate(eventName) as Action<T, U, V>;
        if (cb != null) cb.Invoke(arg1, arg2, arg3);
    }

    public static void ExecuteEvent<T, U, V>(object obj, string eventName, T arg1, U arg2, V arg3) {
        var cb = GetDelegate(obj, eventName) as Action<T, U, V>;
        if (cb != null) cb.Invoke(arg1, arg2, arg3); ;
    }

    public static void ExecuteEvent<T, U, V, W>(string eventName, T arg1, U arg2, V arg3, W arg4) {
        var cb = GetDelegate(eventName) as Action<T, U, V, W>;
        if (cb != null) cb.Invoke(arg1, arg2, arg3, arg4);
    }

    public static void ExecuteEvent<T, U, V, W>(object obj, string eventName, T arg1, U arg2, V arg3, W arg4) {
        var cb = GetDelegate(obj, eventName) as Action<T, U, V, W>;
        if (cb != null) cb.Invoke(arg1, arg2, arg3, arg4);
    }

    public static void UnregisterEvent(string eventName, Action handler) {
        UnregisterEvent(eventName, (Delegate) handler);
    }

    public static void UnregisterEvent(object obj, string eventName, Action handler) {
        UnregisterEvent(obj, eventName, (Delegate) handler);
    }

    public static void UnregisterEvent<T>(string eventName, Action<T> handler) {
        UnregisterEvent(eventName, (Delegate) handler);
    }

    public static void UnregisterEvent<T>(object obj, string eventName, Action<T> handler) {
        UnregisterEvent(obj, eventName, (Delegate) handler);
    }

    public static void UnregisterEvent<T, U>(string eventName, Action<T, U> handler) {
        UnregisterEvent(eventName, (Delegate) handler);
    }

    public static void UnregisterEvent<T, U>(object obj, string eventName, Action<T, U> handler) {
        UnregisterEvent(obj, eventName, (Delegate) handler);
    }

    public static void UnregisterEvent<T, U, V>(string eventName, Action<T, U, V> handler) {
        UnregisterEvent(eventName, (Delegate) handler);
    }

    public static void UnregisterEvent<T, U, V>(object obj, string eventName, Action<T, U, V> handler) {
        UnregisterEvent(obj, eventName, (Delegate) handler);
    }

    public static void UnregisterEvent<T, U, V, W>(string eventName, Action<T, U, V, W> handler) {
        UnregisterEvent(eventName, (Delegate) handler);
    }

    public static void UnregisterEvent<T, U, V, W>(object obj, string eventName, Action<T, U, V, W> handler) {
        UnregisterEvent(obj, eventName, (Delegate) handler);
    }

    private static void RegisterEvent(string eventName, Delegate handler) {
        Delegate value = null;
        if (s_GlobalEventTable.TryGetValue(eventName, out value)) {
            if (value == null || !Array.Exists(value.GetInvocationList(), (Delegate element) => element == handler)) {
                s_GlobalEventTable[eventName] = Delegate.Combine(value, handler);
            }
        }
        else {
            s_GlobalEventTable.Add(eventName, handler);
        }
    }

    private static void RegisterEvent(object obj, string eventName, Delegate handler) {
        Dictionary<string, Delegate> value = null;
        if (!s_EventTable.TryGetValue(obj, out value)) {
            value = new Dictionary<string, Delegate>();
            s_EventTable.Add(obj, value);
        }

        Delegate value2;
        if (value.TryGetValue(eventName, out value2)) {
            value[eventName] = Delegate.Combine(value2, handler);
        }
        else {
            value.Add(eventName, handler);
        }
    }

    private static Delegate GetDelegate(string eventName) {
        Delegate value;
        if (s_GlobalEventTable.TryGetValue(eventName, out value)) {
            return value;
        }
        return null;
    }

    private static Delegate GetDelegate(object obj, string eventName) {
        Dictionary<string, Delegate> value;
        Delegate value2;
        if (s_EventTable.TryGetValue(obj, out value) && value.TryGetValue(eventName, out value2)) {
            return value2;
        }
        return null;
    }

    private static void UnregisterEvent(string eventName, Delegate handler) {
        Delegate value;
        if (s_GlobalEventTable.TryGetValue(eventName, out value)) {
            s_GlobalEventTable[eventName] = Delegate.Remove(value, handler);
        }
    }

    private static void UnregisterEvent(object obj, string eventName, Delegate handler) {
        Dictionary<string, Delegate> value;
        Delegate value2;
        if (s_EventTable.TryGetValue(obj, out value) && value.TryGetValue(eventName, out value2)) {
            value[eventName] = Delegate.Remove(value2, handler);
        }
    }



}
