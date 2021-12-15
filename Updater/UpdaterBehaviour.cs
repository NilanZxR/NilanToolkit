using System;
using UnityEngine;

namespace NilanToolkit.Updater {
    internal class UpdaterBehaviour : MonoBehaviour {
        public event Action OnUpdate;
        public event Action OnLateUpdate;
    }
}