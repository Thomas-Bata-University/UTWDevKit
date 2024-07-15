using UnityEngine;

namespace Script.Utils {
    public class ObjectUtils : MonoBehaviour {

        #region Selectable object

        public static Transform GetReference(Transform tankPart) {
            return tankPart.GetChild(0);
        }

        public static Transform GetCanvas(Transform selectedObject) {
            return selectedObject.parent.GetChild(2);
        }

        public static string GetType(Transform tankPart) {
            return tankPart.parent.GetChild(1).tag;
        }

        public static void SetCanvasVisible(Transform selectedObject) {
            if (selectedObject is null) return;
            var go = GetCanvas(selectedObject).gameObject;
            go.SetActive(!go.activeSelf);
        }

        #endregion

    }
} //END