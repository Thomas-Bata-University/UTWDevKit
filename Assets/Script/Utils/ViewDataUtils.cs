using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Utils {
    public class ViewDataUtils : MonoBehaviour {

        public static TextMeshProUGUI GetName(GameObject data) {
            return data.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        }

        public static Button EditButton(GameObject data) {
            return data.transform.GetChild(2).gameObject.GetComponent<Button>();
        }

        public static Button DeleteButton(GameObject data) {
            return data.transform.GetChild(3).gameObject.GetComponent<Button>();
        }

    }
} //END