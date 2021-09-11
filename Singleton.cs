namespace NilanToolkit {
    public class Singleton<T> where T : new() {

        protected T instance;

        public T Instance {
            get {
                if (instance == null) {
                    instance = new T();
                }
                return instance;
            }
        }
        
    }
}
