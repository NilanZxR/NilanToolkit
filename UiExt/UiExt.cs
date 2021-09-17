using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using UnityEngine;

namespace NilanToolkit.UiExt {
    public static class UiExt {

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
                default: return null;
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
                default: return null;
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
        
    }
}
