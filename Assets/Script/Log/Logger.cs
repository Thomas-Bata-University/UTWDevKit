using System.Collections;
using TMPro;
using UnityEngine;

namespace Script.Log {
    public class Logger : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public static Logger Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI logText;

        private Coroutine _activeMessageCoroutine;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void LogMessage(string message, float duration = 3f) {
            if (_activeMessageCoroutine != null) {
                StopCoroutine(_activeMessageCoroutine);
            }

            logText.text = message;

            _activeMessageCoroutine = StartCoroutine(RemoveMessageAfterTime(duration));
        }

        private IEnumerator RemoveMessageAfterTime(float duration) {
            yield return new WaitForSeconds(duration);
            logText.text = "";
            _activeMessageCoroutine = null;
        }

    }
} //END