namespace NilanToolkit.DataModel {
public class ObservedObject<T> : ObservedObject{
    public T Value {
        get => (T) value;
        set => WriteValue(value);
    }
    
    public ObservedObject(T value = default) {
        this.value = value;
    }

}
}