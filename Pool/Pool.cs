using System;
using System.Collections.Generic;
using UnityEngine;

namespace NilanToolkit.Pool {
    public class Pool {

        public static Dictionary<Type, ObjectPool> GlobalObjectPool = new Dictionary<Type, ObjectPool>();

        public static GameObjectPool GlobalGameObjectPool = new GameObjectPool();


        public static T GetItem<T>() where T : class {
            T item = null;



            return item;
        }

        public static void Collect<T>(T item) {

        }

        private static void GetPool(Type type) {
            if (!GlobalObjectPool.TryGetValue(type, out var pool)) {
                
            }
        }

        private static void CreatePool(Type type) {
            if (typeof(MonoBehaviour).IsAssignableFrom(type)) {

            }
        }

    }
}
