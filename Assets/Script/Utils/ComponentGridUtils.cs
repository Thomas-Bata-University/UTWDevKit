using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Utils {
    public class ComponentGridUtils : MonoBehaviour {

        public static GameObject GetSelectData(GameObject componentGrid) {
            return componentGrid.transform.GetChild(1).gameObject;
        }

        public static TextMeshProUGUI GetName(GameObject data) {
            return data.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        public static GameObject GetContent(GameObject data) {
            return data.transform.GetChild(1).GetChild(0).gameObject;
        }

        public static Button GetSelectButton(GameObject data) {
            return data.transform.GetChild(2).GetComponent<Button>();
        }

        public static Button GetCancelButton(GameObject data) {
            return data.transform.GetChild(3).GetComponent<Button>();
        }

        public static TMP_InputField GetFilterInput(GameObject data) {
            return data.transform.GetChild(4).GetComponent<TMP_InputField>();
        }

    }
} //END