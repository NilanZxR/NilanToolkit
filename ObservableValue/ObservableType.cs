using System;

namespace NilanToolkit.ObservableValue {
    public abstract class ObservableType<T> where T : IEquatable<T> {

        public event ValueChangeEvent<T> OnValueChanged;

        protected T value;
        
        public T Value { 
            get => value;
            set {
                if (value.Equals(this.value)) {
                    return;
                }
                this.value = value;
                OnValueChanged?.Invoke(this.value);
            }
        }
        
        public ObservableType(){ }
        
    }
}
