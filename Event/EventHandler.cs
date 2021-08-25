using System;

namespace NilanToolkit.Event {
    public class EventHandler {

        private Action evt;

        public void Add(Action action) {
            evt += action;
        }

        public void Remove(Action action) {
            evt -= action;
        }

        public void Clear() {
            evt = delegate { };
        }

        public void Invoke() {
            evt?.Invoke();
        }

    }

    public class EventHandler<T> {

        private Action<T> evt;

        public void Add(Action<T> action) {
            evt += action;
        }

        public void Remove(Action<T> action) {
            evt -= action;
        }

        public void Clear() {
            evt = delegate { };
        }

        public void Invoke(T arg) {
            evt?.Invoke(arg);
        }

    }

    public class EventHandler<T1,T2> {

        private Action<T1, T2> evt;

        public void Add(Action<T1, T2> action) {
            evt += action;
        }

        public void Remove(Action<T1, T2> action) {
            evt -= action;
        }

        public void Clear() {
            evt = delegate { };
        }

        public void Invoke(T1 arg1, T2 arg2) {
            evt?.Invoke(arg1,arg2);
        }

    }

    public class EventHandler<T1, T2, T3> {

        private Action<T1, T2, T3> evt;

        public void Add(Action<T1, T2, T3> action) {
            evt += action;
        }

        public void Remove(Action<T1, T2, T3> action) {
            evt -= action;
        }

        public void Clear() {
            evt = delegate { };
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3) {
            evt?.Invoke(arg1, arg2, arg3);
        }

    }

    public class EventHandler<T1, T2, T3, T4> {

        private Action<T1, T2, T3, T4> evt;

        public void Add(Action<T1, T2, T3, T4> action) {
            evt += action;
        }

        public void Remove(Action<T1, T2, T3, T4> action) {
            evt -= action;
        }

        public void Clear() {
            evt = delegate { };
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
            evt?.Invoke(arg1, arg2, arg3, arg4);
        }

    }
}
