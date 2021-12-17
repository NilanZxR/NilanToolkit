using UnityEngine.Events;

namespace NilanToolkit.UnityEventHandler {
    public interface IUnityEventHandler {
        UnityEvent Event { get; }
    }
}