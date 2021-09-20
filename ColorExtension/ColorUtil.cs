using UnityEngine;
using UnityEngine.UI;

namespace NilanToolkit.ColorExtension {
    
    public static class ColorUtil {

        public static float GetOpacity(GameObject target) {
            var graphic = target.GetComponent<Graphic>();
            if (graphic) {
                return graphic.color.a;
            }

            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup) {
                return canvasGroup.alpha;
            }

            // var colorProvider = target.GetComponent<IColorProvider>();
            // if (colorProvider != null) {
            //     return colorProvider.Color.a;
            // }
            //
            // var opacityProvider = target.GetComponent<IOpacityProvider>();
            // if (opacityProvider != null) {
            //     return opacityProvider.Opacity;
            // }

            Debug.LogWarning("not found colorable component");
            return 0f;
        }

        public static void SetOpacity(GameObject target, float value) {
            var graphic = target.GetComponent<Graphic>();
            if (graphic) {
                var color = graphic.color;
                color.a = value;
                graphic.color = color;
                return;
            }

            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup) {
                canvasGroup.alpha = value;
                return;
            }

            // var colorProvider = target.GetComponent<IColorProvider>();
            // if (colorProvider != null) {
            //     var color = colorProvider.Color;
            //     color.a = value;
            //     colorProvider.Color = color;
            //     return;
            // }
            //
            // var opacityProvider = target.GetComponent<IOpacityProvider>();
            // if (opacityProvider != null) {
            //     opacityProvider.Opacity = value;
            //     return;
            // }

            Debug.LogWarning("not found colorable component");
        }

        public static Color GetColor(GameObject target) {
            var graphic = target.GetComponent<Graphic>();
            if (graphic) {
                return graphic.color;
            }

            // var colorProvider = target.GetComponent<IColorProvider>();
            // if (colorProvider != null) {
            //     return colorProvider.Color;
            // }

            Debug.LogWarning("not found colorable component");
            return new Color();
        }

        public static void SetColor(GameObject target, Color col) {
            var graphic = target.GetComponent<Graphic>();
            if (graphic) {
                graphic.color = col;
                return;
            }

            // var colorProvider = target.GetComponent<IColorProvider>();
            // if (colorProvider != null) {
            //     colorProvider.Color = col;
            //     return;
            // }

            Debug.LogWarning("not found colorable component");
        }

        public static string WrapRichTextColorTag(object str, Color color) {
            return WrapRichTextColorTag(str, ColorUtility.ToHtmlStringRGB(color));
        }

        public static string WrapRichTextColorTag(object str, string hexColorCode) {
            return $"<color=#{hexColorCode}>{str}</color>";
        }
        

    }
}
