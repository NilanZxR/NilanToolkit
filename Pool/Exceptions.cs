using System;

namespace NilanToolkit.Pool {
    public class PoolingException : Exception {

        public PoolingException(string message) : base(message){}

    }

    public class PoolNotFoundException : PoolingException {

        public PoolNotFoundException(string poolId) : base($"Pool not found: {poolId}") { }
    }
    
}