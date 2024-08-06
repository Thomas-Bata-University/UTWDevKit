using TMPro;
using UnityEngine;
using Logger = Script.Log.Logger;

namespace Script.SceneLoad {
    public class LoadingScreen : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public static LoadingScreen Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI logText;
        [SerializeField] private GameObject loadingScreen;

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

        public void Show() {
            loadingScreen.SetActive(true);
        }

        public void Hide() {
            loadingScreen.SetActive(false);
        }

        public void SetText(string text) {
            logText.text = text;
        }

    }
} //END