using System;
using UnityEngine;
using UnityEngine.UI;

namespace NilanToolkit.DataBinding {
    
    [RequireComponent(typeof(Button))]
    public class DataBinding_Button : DataBindingComponentBase {

        public string bindInteractive;
        
        private void Start() {
            var model = FindDataModel();
            var button = GetComponent<Button>();
            
            model.AddDataListener(bindInteractive, () => {
                button.interactable = model.GetValue<bool>(bindInteractive);
            });
        }
    }
}