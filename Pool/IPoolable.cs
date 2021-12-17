namespace NilanToolkit.Pool {
    
    /// <summary>
    /// receive callback when pooling action 
    /// </summary>
    public interface IPoolEventHandler {

        /// <summary>
        /// if register loader to pool, this callback will called when object create
        /// </summary>
        void OnCreate();

        /// <summary>
        /// call when object be put into pool
        /// </summary>
        void OnCollect();

        /// <summary>
        /// call when object take out from pool
        /// </summary>
        void OnReuse();

    }
    
}
