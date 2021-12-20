using System;
using UnityEngine;

namespace NilanToolkit.Selection {
    [RequireComponent(typeof(Camera))]
    public class CameraSelectionDetector : MonoBehaviour{
    
        private Camera _cam;
        
        private void Start() {
            _cam = GetComponent<Camera>();
        }

        private void Update() {
            if (Input.touchCount > 1) {
                return;
            }

            if (!IsPointerDown()) return;
            if (RayCast(out var selection)) {
                Selection.Select(selection);
            }
            else {
                Selection.Select(null);
            }
        }

        private bool RayCast(out ISelection selection) {
            var mousePosition = Input.mousePosition;
            var ray = _cam.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out var hit)) {
                selection = null;
                return false;
            }

            selection = hit.collider.GetComponent<ISelection>();
            if (selection != null) return true;
            
            var selectionProvider = hit.collider.GetComponent<SelectionProvider>();
            if (!selectionProvider || !selectionProvider.target) return false;
            selection = selectionProvider.target.GetComponent<ISelection>();
            return selection != null;
        }

        private static bool IsPointerDown() {
            if (Input.GetMouseButtonDown(0)) return true;
            if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Began) return true;
            return false;
        }

        private bool IsPointerUp() {
            if (Input.GetMouseButtonUp(0)) return true;
            if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Ended) return true;
            return false;
        }
    }
}