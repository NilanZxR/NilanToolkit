using UnityEngine;
using Color=UnityEngine.Color;

namespace NilanToolkit.UiExtensions {

    public interface IColorProvider {
        Color Color { get; set; }
    }

}
