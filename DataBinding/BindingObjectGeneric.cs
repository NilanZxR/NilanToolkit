namespace NilanToolkit.DataBinding {
public class BindingObject<T> : BindingObject{
    public T Value {
        get => (T) value;
        set => WriteValue(value);
    }
    
    public BindingObject(T value = default) {
        this.value = value;
    }

}
}