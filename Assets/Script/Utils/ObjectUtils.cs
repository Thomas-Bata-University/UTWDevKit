using UnityEngine;

namespace Script.Utils {
    public class ObjectUtils : MonoBehaviour {

        #region Selectable object

        /// <summary>
        /// Get selectable object.
        /// </summary>
        /// <param name="tankPart"></param>
        /// <returns></returns>
        public static Transform GetReference(Transform tankPart) {
            return tankPart.GetChild(0);
        }

        public static Transform GetTag(Transform tankPart) {
            return tankPart.GetChild(1);
        }

        public static Transform GetCanvas(Transform selectedObject) {
            return selectedObject.parent.GetChild(2);
        }

        public static GameObject GetArrows(Transform selectedObject) {
            return GetCanvas(selectedObject).GetChild(0).gameObject;
        }

        public static GameObject GetTorus(Transform selectedObject) {
            return GetCanvas(selectedObject).GetChild(1).gameObject;
        }

        public static void EnableArrows(Transform selectedObject) {
            GetArrows(selectedObject).SetActive(true);
            GetTorus(selectedObject).SetActive(false);
        }

        public static void EnableTorus(Transform selectedObject) {
            GetArrows(selectedObject).SetActive(false);
            GetTorus(selectedObject).SetActive(true);
        }

        public static string GetType(Transform tankPart) {
            return tankPart.parent.GetChild(1).tag;
        }

        public static void SetCanvasVisible(Transform selectedObject) {
            if (selectedObject is null) return;
            var go = GetCanvas(selectedObject).gameObject;
            go.SetActive(!go.activeSelf);
        }

        public static Vector3 GetObjectCenter(Transform selectedObject) {
            return selectedObject.GetComponent<Renderer>().bounds.center;
        }

        #endregion

    }
} //END