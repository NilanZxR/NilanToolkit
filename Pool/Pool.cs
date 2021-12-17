using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NilanToolkit.Pool {
    /// <summary>
    /// A object-pool tools, help you manage, pooling, and reuse object without re-instantiate
    /// </summary>
    public static class Pool {

        private static readonly Dictionary<string, GameObjectPool> Pools = new Dictionary<string, GameObjectPool>();
        
        /// <summary>
        /// get object by pool-id, return null if pool not exist
        /// </summary>
        /// <param name="poolId"></param>
        /// <returns></returns>
        public static GameObject Get(string poolId) {
            if (!IsPoolExist(poolId)) {
                return null;
            }

            var pool = GetPool(poolId);
            var obj = pool.GetItem();
            return obj;
        }

        /// <summary>
        /// get item by Type.Name, return null if pool not exist
        /// </summary>
        public static T Get<T>() where T : Component {
            var poolId = GetTypeName<T>();
            var obj = Get(poolId);
            return obj.GetComponent<T>();
        }

        /// <summary>
        /// get multiple objects by id, throw exception if pool not exist
        /// </summary>
        /// <param name="poolId">id</param>
        /// <param name="count">how many objects be create</param>
        /// <returns></returns>
        /// <exception cref="PoolNotFoundException"></exception>
        public static GameObject[] GetRange(string poolId, int count) {
            if (!IsPoolExist(poolId)) {
                throw new PoolNotFoundException(poolId);
            }
            var arr = new GameObject[count];
            for (var i = 0; i < count; i++) {
                arr[i] = Get(poolId);
            }

            return arr;
        }

        /// <summary>
        /// get multiple objects by Type.Name, throw exception if pool not exist
        /// </summary>
        /// <param name="count">how many objects be create</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="PoolNotFoundException"></exception>
        public static T[] GetRange<T>(int count) where T : Component {
            if (!IsPoolExist<T>()) {
                throw new PoolNotFoundException(typeof(T).Name);
            }
            var poolId = GetTypeName<T>();
            var arr = new T[count];
            for (var i = 0; i < count; i++) {
                var go = Get(poolId);
                arr[i] = go.GetComponent<T>();
            }

            return arr;
        }

        /// <summary>
        /// collect item by id, auto create pool if not exist
        /// </summary>
        /// <param name="poolId"></param>
        /// <param name="item"></param>
        public static void Collect(string poolId, GameObject item) {
            if (!IsPoolExist(poolId)) {
                var loader = item.GetComponent<IGameObjectLoader>();
                if (loader != null) {
                    CreatePool(poolId, loader.Create);
                }
                else {
                    CreatePool(poolId);
                }
            }
            var pool = GetPool(poolId);
            pool.Collect(item);
        }

        /// <summary>
        /// Collect item by Type.Name, auto create pool if not exist
        /// </summary>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        public static void Collect<T>(T item) where T : Component{
            var poolId = GetTypeName<T>();
            Collect(poolId, item.gameObject);
        }

        /// <summary>
        /// collect multiple objects by Type.Name, auto create pool if not exist
        /// </summary>
        /// <param name="items"></param>
        /// <typeparam name="T"></typeparam>
        public static void CollectRange<T>(IEnumerable<T> items) where T : Component {
            foreach (var item in items) {
                Collect(item);
            }
        }

        /// <summary>
        /// get pool by id, return null if not exist
        /// </summary>
        /// <param name="poolId"></param>
        /// <returns></returns>
        public static GameObjectPool GetPool(string poolId) {
            Pools.TryGetValue(poolId, out var pool);
            return pool;
        }
        
        /// <summary>
        /// get a pool by id, or create it if not exist
        /// </summary>
        /// <param name="poolId"></param>
        /// <returns></returns>
        public static GameObjectPool GetOrCreatePool(string poolId) {
            if (!IsPoolExist(poolId)) {
                CreatePool(poolId);
            }
            var pool = GetPool(poolId);
            return pool;
        }

        /// <summary>
        /// get a pool by Type.Name, or create it if not exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static GameObjectPool GetOrCreatePool<T>(){
            var poolId = GetTypeName<T>();
            return GetOrCreatePool(poolId);
        }

        /// <summary>
        /// check pool exist by id
        /// </summary>
        /// <param name="poolId"></param>
        /// <returns></returns>
        public static bool IsPoolExist(string poolId) {
            return Pools.ContainsKey(poolId);
        }

        /// <summary>
        /// check pool exist by Type.Name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsPoolExist<T>(){
            var poolId = GetTypeName<T>();
            return IsPoolExist(poolId);
        }
        
        /// <summary>
        /// create a pool by id
        /// </summary>
        /// <param name="poolId">a unique id</param>
        public static void CreatePool(string poolId) {
            if (IsPoolExist(poolId)) {
                throw new PoolingException("duplicate pool id in global table");
            }

            var pool = new GameObjectPool();
            Pools[poolId] = pool;
        }

        /// <summary>
        /// create a pool by Type.Name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void CreatePool<T>(){
            CreatePool(GetTypeName<T>());
        }

        /// <summary>
        /// create a pool by id
        /// </summary>
        /// <param name="poolId"></param>
        /// <param name="loader"></param>
        /// <exception cref="PoolingException"></exception>
        public static void CreatePool(string poolId, Loader<GameObject> loader) {
            if (IsPoolExist(poolId)) {
                throw new PoolingException("duplicate pool id in global table");
            }

            var pool = new GameObjectPool(loader);
            Pools[poolId] = pool;
        }

        /// <summary>
        /// create a pool by Type.Name
        /// </summary>
        /// <param name="loader"></param>
        /// <typeparam name="T"></typeparam>
        public static void CreatePool<T>(Loader<GameObject> loader) {
            var poolId = GetTypeName<T>();
            CreatePool(poolId, loader);
        }
        
        /// <summary>
        /// create a pool by id
        /// </summary>
        /// <param name="poolId"></param>
        /// <param name="loader"></param>
        /// <param name="preload"></param>
        public static void CreatePool(string poolId, Loader<GameObject> loader, int preload) {
            CreatePool(poolId, loader);
            var pool = Pools[poolId];
            pool.Preload(preload);
        }

        /// <summary>
        /// create a pool by Type.Name
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="preload"></param>
        /// <typeparam name="T"></typeparam>
        public static void CreatePool<T>(Loader<GameObject> loader, int preload) {
            var poolId = GetTypeName<T>();
            CreatePool(poolId, loader, preload);
        }

        /// <summary>
        /// create a pool with a auto-generated guid
        /// </summary>
        /// <returns>pool id(guid)</returns>
        public static string CreatePool() {
            var guid = System.Guid.NewGuid().ToString("N");
            CreatePool(guid);
            return guid;
        }

        /// <summary>
        /// Remove pool by id
        /// </summary>
        /// <param name="poolId"></param>
        /// <param name="disposeAllItem">if true, all item will be destroyed</param>
        public static void RemovePool(string poolId, bool disposeAllItem = true) {
            if (!IsPoolExist(poolId)) {
                throw new PoolNotFoundException(poolId);
            }

            var pool = GetPool(poolId);
            if (disposeAllItem) {
                pool.Dispose();
            }
            else {
                pool.stack.Clear();
            }

            Pools.Remove(poolId);
        }

        /// <summary>
        /// Remove pool by Type.Name
        /// </summary>
        /// <param name="disposeAllItem">if true, all item will be destroyed</param>
        /// <typeparam name="T"></typeparam>
        public static void RemovePool<T>(bool disposeAllItem = true) {
            var poolId = GetTypeName<T>();
            RemovePool(poolId, disposeAllItem);
        }

        /// <summary>
        /// match the pool which it does first item passed
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static GameObjectPool FindPoolByFirstItem(Predicate<GameObject> predicate) {
            foreach (var pair in Pools) {
                var pool = pair.Value;
                var obj = pool.stack.Peek();
                if (predicate(obj)) {
                    return pool;
                }
            }

            return null;
        }

        /// <summary>
        /// match the pool which it does all item passed
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static GameObjectPool FindPoolByAllItem(Predicate<GameObject> predicate) {
            foreach (var pair in Pools) {
                var pool = pair.Value;
                if (pool.stack.All(i=>predicate(i))) {
                    return pool;
                }
            }

            return null;
        }

        /// <summary>
        /// match the pool which it does any item passed
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static GameObjectPool FindPoolByAnyItem(Predicate<GameObject> predicate) {
            foreach (var pair in Pools) {
                var pool = pair.Value;
                if (pool.stack.Any(i=>predicate(i))) {
                    return pool;
                }
            }

            return null;
        }

        /// <summary>
        /// clear all pools and pooled items, if you want to keep items alive, use ClearAndKeepItems()
        /// </summary>
        public static void Clear() {
            foreach (var pair in Pools) {
                var gameObjectPool = pair.Value;
                gameObjectPool.Dispose();
            }
            Pools.Clear();
        }

        /// <summary>
        /// clear all pools, and keep objects alive
        /// </summary>
        public static void ClearAndKeepItems() {
            foreach (var pair in Pools) {
                var gameObjectPool = pair.Value;
                gameObjectPool.DisposeAndKeepItemsAlive();
            }
            Pools.Clear();
        }

        private static string GetTypeName<T>() {
            return typeof(T).Name;
        }

    }
}
