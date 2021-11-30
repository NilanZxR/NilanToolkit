using UnityEngine;

namespace NilanToolkit.DataBinding {
    public class DataBindingComponentBase : MonoBehaviour {

        [SerializeField] private GameObject dataModelProvider;

        protected DataModel FindDataModel() {
            DataModel model = null;
            if (dataModelProvider) {
                var provider = dataModelProvider.GetComponent<IDataModelProvider>();
                if (provider != null) model = provider.Model;
            }
            else {
                var provider = GetComponentInParent<IDataModelProvider>();
                if (provider != null) {
                    model = provider.Model;
                }
            }

            return model;
        }

    }
}