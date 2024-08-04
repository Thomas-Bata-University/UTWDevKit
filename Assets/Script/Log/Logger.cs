using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

            SceneManager.activeSceneChanged += StopCoroutine;
        }

        public void LogMessage(string message, float duration = 3f) {
            Message(message, duration);
        }

        public void LogSuccessfulMessage(string message, float duration = 3f) {
            Message(message, duration, Color.green);
        }

        public void LogErrorMessage(string message, float duration = 3f) {
            Message(message, duration, Color.red);
        }

        private void Message(string message, float duration, Color color = default) {
            if (color == default) color = Color.white;

            StopCoroutine();

            logText.text = message;
            logText.color = color;
            _activeMessageCoroutine = StartCoroutine(RemoveMessageAfterTime(duration));
        }

        private IEnumerator RemoveMessageAfterTime(float duration) {
            yield return new WaitForSeconds(duration);
            logText.text = "";
            _activeMessageCoroutine = null;
        }

        private void StopCoroutine() {
            if (_activeMessageCoroutine != null) {
                StopCoroutine(_activeMessageCoroutine);
            }
        }

        private void StopCoroutine(Scene oldScene, Scene newScene) {
            StopCoroutine();
            logText.text = null;
        }

        private void OnDestroy() {
            SceneManager.activeSceneChanged -= StopCoroutine;
        }

    }
} //END