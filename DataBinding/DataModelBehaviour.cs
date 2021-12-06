using System;
using UnityEngine;

namespace NilanToolkit.DataBinding {
    public class DataModelBehaviour : MonoBehaviour {

        public event Action OnLateUpdate;

        private void LateUpdate() {
            OnLateUpdate?.Invoke();
        }
    }
}