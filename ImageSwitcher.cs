using System;
using System.Collections.Generic;
using NilanToolkit.CSharpExtensions;
using UnityEngine;
using UnityEngine.UI;

namespace NilanToolkit {
    
    [RequireComponent(typeof(Image))]
    public class ImageSwitcher : MonoBehaviour{

        [Serializable]
        public class Option {
            public string key;
            public Sprite sprite;
        }

        public string defaultKey;
        public List<Option> options;
        public Image image;
        public Option Current { get; private set; }

        private void Awake() {
            image = GetComponent<Image>();
            if (defaultKey.NotNullOrEmpty()) {
                SetImage(defaultKey);
            }
        }

        public void SetImage(string key) {
            var option = options.Find(i=> i.key == key);
            if (image == null) image = GetComponent<Image>();
            if (option == null) {
                image.sprite = null;
                Current = null;
            }
            else {
                image.sprite = option.sprite;
                Current = option;
            }
        }

    }
    
}

namespace NilanToolkit.UiExtensions {
    public static partial class UiExt {

        public static string GetImageKey(Component component) {
            if (component is ImageSwitcher imageSwitcher) {
                if (imageSwitcher.Current != null) {
                    return imageSwitcher.Current.key;
                }
            }

            return null;
        }

        public static string GetImageKey(this Transform transform, string path) {
            var go = transform.Find(path).gameObject;
            return GetProperty(go, GetImageKey);
        }

        public static void SetImageKey(Component component, string key) {
            if (component is ImageSwitcher imageSwitcher) {
                imageSwitcher.SetImage(key);
            }
        }

        public static void SetImageKey(this Transform transform, string path, string key) {
            var go = transform.Find(path).gameObject;
            SetProperty(go, key, SetImageKey);
        }
        
    }
}
