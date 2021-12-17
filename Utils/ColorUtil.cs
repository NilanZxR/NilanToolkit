using UnityEngine;
using UnityEngine.UI;

namespace NilanToolkit.Utils {
    
    public static class ColorUtil {

        public static string WrapRichTextColorTag(object str, Color color) {
            return WrapRichTextColorTag(str, ColorUtility.ToHtmlStringRGB(color));
        }

        public static string WrapRichTextColorTag(object str, string hexColorCode) {
            return $"<color=#{hexColorCode}>{str}</color>";
        }
        

    }
}
