using UnityEngine;
using Color=UnityEngine.Color;

namespace NilanToolkit.ColorExtension {

    public interface IColorProvider {
        Color Color { get; set; }
    }

}
