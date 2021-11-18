using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NilanToolkit.Pool {
    public class Pool {

        private static readonly Dictionary<string, GameObjectPool> Global = new Dictionary<string, GameObjectPool>();
        
        public static GameObject Get(string poolId) {
            if (!IsPoolExist(poolId)) {
                return null;
            }

            var pool = GetPool(poolId);
            var obj = pool.GetItem();
            return obj;
        }

        /// <summary>
        /// the type name(typeof(T).Name) will used to poolId
        /// </summary>
        public static T Get<T>(T item) where T : Component {
            var poolId = GetTypeName<T>();
            var obj = Get(poolId);
            return obj.GetComponent<T>();
        }

        /// <summary>
        /// auto create pool, and collect item
        /// </summary>
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
        /// the type name(typeof(T).Name) will used to poolId
        /// </summary>
        public static void Collect<T>(T item) where T : Component{
            var poolId = GetTypeName<T>();
            Collect(poolId, item.gameObject);
        }

        public static GameObjectPool GetPool(string poolId) {
            Global.TryGetValue(poolId, out var pool);
            return pool;
        }
        
        /// <summary>
        /// get a pool by id, or create it if not exist
        /// </summary>
        /// <param name="poolId"></param>
        /// <returns></returns>
        public static GameObjectPool SafeGetPool(string poolId) {
            if (!IsPoolExist(poolId)) {
                CreatePool(poolId);
            }
            var pool = GetPool(poolId);
            return pool;
        }

        public static bool IsPoolExist(string poolId) {
            return Global.ContainsKey(poolId);
        }
        
        /// <summary>
        /// create a pool
        /// </summary>
        /// <param name="poolId">a unique id</param>
        public static void CreatePool(string poolId) {
            if (IsPoolExist(poolId)) {
                throw new PoolingException("duplicate pool id in global table");
            }

            var pool = new GameObjectPool();
            Global[poolId] = pool;
        }

        public static void CreatePool(string poolId, Loader<GameObject> loader) {
            if (IsPoolExist(poolId)) {
                throw new PoolingException("duplicate pool id in global table");
            }

            var pool = new GameObjectPool(loader);
            Global[poolId] = pool;
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

            Global.Remove(poolId);
        }

        public static void RemovePool<T>(bool disposeAllItem = true) {
            var poolId = GetTypeName<T>();
            RemovePool(poolId, disposeAllItem);
        }

        /// <summary>
        /// match first item in pool and return first passed pool
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static GameObjectPool FindPoolByFirstItem(Predicate<GameObject> predicate) {
            foreach (var pair in Global) {
                var pool = pair.Value;
                var obj = pool.stack.Peek();
                if (predicate(obj)) {
                    return pool;
                }
            }

            return null;
        }

        /// <summary>
        /// match all item in pool and return first passed pool
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static GameObjectPool FindPoolByAllItem(Predicate<GameObject> predicate) {
            foreach (var pair in Global) {
                var pool = pair.Value;
                if (pool.stack.All(i=>predicate(i))) {
                    return pool;
                }
            }

            return null;
        }

        /// <summary>
        /// clear all pools and pooled items, if you want to keep items alive, use ClearNoDispose()
        /// </summary>
        public static void Clear() {
            foreach (var pair in Global) {
                var gameObjectPool = pair.Value;
                gameObjectPool.Dispose();
            }
            Global.Clear();
        }

        public static void ClearNoDispose() {
            Global.Clear();
        }

        private static string GetTypeName<T>() {
            return typeof(T).Name;
        }

    }
}
