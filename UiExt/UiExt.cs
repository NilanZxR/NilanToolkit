using System;
using UnityEngine;

namespace NilanToolkit.UiExtensions {
    public static partial class UiExt {

        public static T GetProperty<T>(GameObject go, Func<Component, T> getter) {
            var components = go.GetComponents<Component>();
            foreach (var component in components) {
                var result = getter(component);
                if (result != null) return result;
            }
            return default;
        }

        public static void SetProperty<T>(GameObject go, T t, Action<Component, T> setter) {
            var components = go.GetComponents<Component>();
            foreach (var component in components) {
                setter(component, t);
            }
        }

        #region Text (string)
        
        public static string GetText(Component component) {
            switch (component) {
                case UnityEngine.UI.Text text: {
                    return text.text;
                }
                case UnityEngine.UI.InputField inputField: {
                    return inputField.text;
                }
#if USING_TEXTMESHPRO
                case TMPro.TMP_Text text: {
                    return text.text;
                }
                case TMPro.TMP_InputField tmpInputField: {
                    return tmpInputField.text;
                }
#endif
                case ITextProvider textProvider: {
                    return textProvider.Text;
                }
                default: return default;
            }
        }

        public static string GetText(string path) {
            var go = GameObject.Find(path);
            return GetProperty(go, GetText);
        }

        public static string GetText(this Transform transform, string path) {
            var go = transform.Find(path).gameObject;
            return GetProperty(go, GetText);
        }
        
        public static void SetText(Component component, string content) {
            switch (component) {
                case UnityEngine.UI.Text text: {
                    text.text = content;
                    return;
                }
                case UnityEngine.UI.InputField inputField: {
                    inputField.text = content;
                    return;
                }
#if USING_TEXTMESHPRO
                case TMPro.TMP_Text text: {
                    text.text = content;
                    return;
                }
                case TMPro.TMP_InputField tmpInputField: {
                    tmpInputField.text = content;
                    return;
                }
#endif
                case ITextProvider textProvider: {
                    textProvider.Text = content;
                    return;
                }
            }
        }

        public static void SetText(string path, string content) {
            var go = GameObject.Find(path);
            SetProperty(go, content, SetText);
        }

        public static void SetText(this Transform transform, string path, string content) {
            var go = transform.Find(path).gameObject;
            SetProperty(go, content, SetText);
        }
        
        #endregion

        #region Sprite
        
        public static Sprite GetSprite(Component component) {
            switch (component) {
                case UnityEngine.UI.Image image: {
                    return image.sprite;
                }
                case SpriteRenderer spriteRenderer: {
                    return spriteRenderer.sprite;
                }
                case ISpriteProvider spriteProvider: {
                    return spriteProvider.Sprite;
                }
                default: return default;
            }
        }

        public static Sprite GetSprite(string path) {
            var go = GameObject.Find(path);
            return GetProperty(go, GetSprite);
        }

        public static Sprite GetSprite(this Transform transform, string path) {
            var go = transform.Find(path).gameObject;
            return GetProperty(go, GetSprite);
        }

        public static void SetSprite(Component component, Sprite sprite) {
            switch (component) {
                case UnityEngine.UI.Image image: {
                    image.sprite = sprite;
                    break;
                }
                case SpriteRenderer spriteRenderer: {
                    spriteRenderer.sprite = sprite;
                    break;
                }
                case ISpriteProvider spriteProvider: {
                    spriteProvider.Sprite = sprite;
                    break;
                }
            }
        }

        public static void SetSprite(string path, Sprite sprite) {
            var go = GameObject.Find(path);
            SetProperty(go, sprite, SetSprite);
        }
        
        public static void SetSprite(this Transform transform, string path, Sprite sprite) {
            var go = transform.Find(path).gameObject;
            SetProperty(go, sprite, SetSprite);
        }
        
        #endregion

        #region Color
        
        public static Color GetColor(Component component) {
            switch (component) {
                case UnityEngine.UI.Graphic graphic: {
                    return graphic.color;
                }
                case IColorProvider colorProvider: {
                    return colorProvider.Color;
                }
                default: return default;
            }
        }

        public static Color GetColor(string path) {
            var go = GameObject.Find(path);
            return GetProperty(go, GetColor);
        }

        public static Color GetColor(this Transform transform, string path) {
            var go = transform.Find(path).gameObject;
            return GetProperty(go, GetColor);
        }
        
        public static void SetColor(Component component, Color color) {
            switch (component) {
                case UnityEngine.UI.Graphic graphic: {
                    graphic.color = color;
                    return;
                }
                case IColorProvider colorProvider: {
                    colorProvider.Color = color;
                    return;
                }
            }
        }

        public static void SetColor(string path, Color color) {
            var go = GameObject.Find(path);
            SetProperty(go, color, SetColor);
        }

        public static void SetColor(this Transform transform, string path, Color color) {
            var go = transform.Find(path).gameObject;
            SetProperty(go, color, SetColor);
        }
        
        #endregion

        #region Opacity
        
        public static float GetOpacity(Component component) {
            switch (component) {
                case UnityEngine.UI.Graphic graphic: {
                    return graphic.color.a;
                }
                case UnityEngine.CanvasGroup canvasGroup: {
                    return canvasGroup.alpha;
                }
                case IOpacityProvider opacityProvider: {
                    return opacityProvider.Opacity;
                }
                default: return default;
            }
        }

        public static float GetOpacity(string path) {
            var go = GameObject.Find(path);
            return GetProperty(go, GetOpacity);
        }

        public static float GetOpacity(this Transform transform) {
            return GetProperty(transform.gameObject, GetOpacity);
        }

        public static float GetOpacity(this Transform transform, string path) {
            var go = transform.Find(path).gameObject;
            return GetProperty(go, GetOpacity);
        }
        
        public static void SetOpacity(Component component, float opacity) {
            switch (component) {
                case UnityEngine.UI.Graphic graphic: {
                    var color = graphic.color;
                    color.a = opacity;
                    graphic.color = color;
                    break;
                }
                case UnityEngine.CanvasGroup canvasGroup: {
                    canvasGroup.alpha = opacity;
                    break;
                }
                case IOpacityProvider opacityProvider: {
                    opacityProvider.Opacity = opacity;
                    break;
                }
            }
        }

        public static void SetOpacity(string path, float opacity) {
            var go = GameObject.Find(path);
            SetProperty(go, opacity, SetOpacity);
        }

        public static void SetOpacity(this Transform transform, float opacity) {
            SetProperty(transform.gameObject, opacity, SetOpacity);
        }

        public static void SetOpacity(this Transform transform, string path, float opacity) {
            var go = transform.Find(path).gameObject;
            SetProperty(go, opacity, SetOpacity);
        }
        
        #endregion

        #region Active

        public static void SetActive(this Transform transform, string path, bool active) {
            var find = transform.Find(path).gameObject;
            find.SetActive(active);
        }

        public static bool GetActive(this Transform transform, string path) {
            var find = transform.Find(path).gameObject;
            return find.activeSelf;
        }
        
        #endregion
        
    }
}
