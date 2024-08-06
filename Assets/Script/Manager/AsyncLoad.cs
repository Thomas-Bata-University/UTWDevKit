using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Manager {
    public class AsyncLoad : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public static AsyncLoad Instance;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }

        public void Load(string sceneName) {
            StartCoroutine(LoadTargetSceneAsync(sceneName));
        }

        private IEnumerator LoadTargetSceneAsync(string sceneName) {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            SceneLoad.LoadingScreen.Instance.Show();

            while (!asyncLoad.isDone) {
                // Update progress
                Debug.Log("Loading... " + (asyncLoad.progress * 100) + "%");

                // Check if the load has finished
                if (asyncLoad.progress >= 0.9f) {
                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }
        }

    }
} //END