using System;
using UnityEngine;
using UnityEngine.UI;

namespace NilanToolkit.DataBinding {
    [RequireComponent(typeof(Text))]
    public class DataBinding_Text : DataBindingComponentBase {

        public string contentBind;
        
        private Text _text;
        
        private void Start() {
            var model = FindDataModel();
            if (model == null) return;
            
            _text = GetComponent<Text>();
            model.AddDataListener(contentBind, () => {
                _text.text = model.GetValue<string>(contentBind);
            });
            
        }
    }
}