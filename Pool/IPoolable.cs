namespace NilanToolkit.Pool {
    public interface IPoolableObject {

        void OnCollect();

        void OnReuse();

    }
}
