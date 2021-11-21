namespace NilanToolkit.Pool {
    public interface IObjectLoader<out T> {

        T Create();

    }

    public interface IGameObjectLoader : IObjectLoader<UnityEngine.GameObject> {
        
    }

}
