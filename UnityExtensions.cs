using UnityEngine;

namespace NilanToolkit {
    public static class UnityExtensions {

        #region GameObject

        public static T AddOrGetComponent<T>(this GameObject target) where T : Component {
            var component = target.GetComponent<T>();
            if (component == null) component = target.AddComponent<T>();
            return component;
        }

        #endregion

        #region Vector

        public static bool IsNearly(this Vector2 target, Vector2 other, float precision = 0.1f) {
            return Mathf.Abs(target.x - other.x) < precision &&
                   Mathf.Abs(target.y - other.y) < precision;
        }

        public static bool IsNearly(this Vector3 target, Vector3 other, float precision = 0.1f) {
            return Mathf.Abs(target.x - other.x) < precision &&
                   Mathf.Abs(target.y - other.y) < precision &&
                   Mathf.Abs(target.z - other.z) < precision;
        }

        public static bool IsNearly(this Vector4 target, Vector4 other, float precision = 0.1f) {
            return Mathf.Abs(target.x - other.x) < precision &&
                   Mathf.Abs(target.y - other.y) < precision &&
                   Mathf.Abs(target.z - other.z) < precision &&
                   Mathf.Abs(target.w - other.w) < precision;
        }

        #endregion

    }
}
