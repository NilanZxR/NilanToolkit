namespace NilanToolkit.ObservableValue {
    public class ObservableInt : ObservableType<int> {

        public ObservableInt(int i) {
            value = i;
        }

        public static implicit operator int(ObservableInt i) {
            return i.Value;
        }
        
        public static implicit operator ObservableInt(int i) {
            return new ObservableInt(i);
        }
        
        public static int operator +(ObservableInt a, ObservableInt b) {
            return a.Value + b.Value;
        }

        public static int operator -(ObservableInt a, ObservableInt b) {
            return a.Value - b.Value;
        }
        
        public static int operator *(ObservableInt a, ObservableInt b) {
            return a.Value * b.Value;
        }
        
        public static int operator /(ObservableInt a, ObservableInt b) {
            return a.Value / b.Value;
        }
        
    }
}
