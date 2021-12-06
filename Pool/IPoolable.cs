namespace NilanToolkit.Pool {
    
    /// <summary>
    /// 通过实现此接口，可接收一些池化生命周期的回调。不实现也能用
    /// </summary>
    public interface IPoolableObject {

        /// <summary>
        /// 物体被创建时调用
        /// </summary>
        void OnCreate();

        /// <summary>
        /// 物体被回收时调用
        /// </summary>
        void OnCollect();

        /// <summary>
        /// 物体被重用时调用
        /// </summary>
        void OnReuse();

    }
    
}
