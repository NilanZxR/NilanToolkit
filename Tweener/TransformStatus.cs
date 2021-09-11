using System;
using UnityEngine;

namespace NilanToolkit.Tweener {
    [Serializable]
    public struct TransformStatus {
        public enum PositionType {
            Global,
            Local
        }

        public PositionType type;

        public Vector3 position;

        public Vector3 scale;

        public Vector3 rotation;

        public static TransformStatus Create(Transform source, PositionType positionType = PositionType.Local)
        {
            var ret = new TransformStatus();
            ret.type = positionType;
            switch (positionType) {
            case PositionType.Global:
                ret.position = source.position;
                break;
            case PositionType.Local:
                ret.position = source.localPosition;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(positionType), positionType, null);
            }

            ret.scale = source.localScale;

            ret.rotation = source.rotation.eulerAngles;
            return ret;
        }

        public static TransformStatus operator +(TransformStatus x, TransformStatus y)
        {
            x.position += y.position;
            x.scale += y.scale;
            x.rotation += y.rotation;
            return x;
        }

        public static TransformStatus operator -(TransformStatus x, TransformStatus y)
        {
            x.position -= y.position;
            x.scale -= y.scale;
            x.rotation -= y.rotation;
            return x;
        }

        public static TransformStatus Lerp(TransformStatus a, TransformStatus b, float t)
        {
            var position = Vector3.Lerp(a.position, b.position, t);
            var scale = Vector3.Lerp(a.scale, b.scale, t);
            var rotation = Vector3.Lerp(a.rotation, b.rotation, t);
            return new TransformStatus() {
                type = a.type,
                position = position,
                scale = scale,
                rotation = rotation
            };
        }

        public void Apply(Transform target)
        {
            switch (type) {
            case PositionType.Global:
                target.position = position;
                break;
            case PositionType.Local:
                target.localPosition = position;
                break;
            }

            target.localScale = scale;
            target.rotation = Quaternion.Euler(rotation);
        }
    }

}
