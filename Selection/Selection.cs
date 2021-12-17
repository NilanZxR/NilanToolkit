using System;
using UnityEngine;

namespace NilanToolkit.Selection {
    public static class Selection {

        private static ISelection _currentSelected;

        public static ISelection CurrentSelected {
            get => _currentSelected;
            set => Select(value);
        }

        public static Action onSelectItemChanged;

        public static bool IsSelected(this ISelection target) {
            return CurrentSelected == target;
        }

        public static void Select(ISelection select) {
            if (_currentSelected == select) return;
            var old = _currentSelected;
            _currentSelected = select;
            
            // unselect old element
            if (old != null && ((Component) old).gameObject) {
                old.IsSelect = false;
                old.OnDeselect();
            }

            // select new element
            if (_currentSelected != null) {
                _currentSelected.IsSelect = true;
                _currentSelected.OnSelect();
            }
            
            //send event
            onSelectItemChanged?.Invoke();
        }



    }
}