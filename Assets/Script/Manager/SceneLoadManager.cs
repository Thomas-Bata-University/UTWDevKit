using System.Collections;
using Script.Controller;
using Script.Enum;
using Script.SceneLoad;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Script.Manager {
    public class SceneLoadManager : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public static SceneLoadManager Instance;

        public static UnityAction OnPreviousSceneLoad;

        private string _fileName; //Without extension
        private Scene _previewScene;
        private Transform _partTypeParent; //MAIN part parent
        private TankPartType _partType;

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

            LoadingScreen.Instance.Show();

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

        #region New scene

        public void LoadNewScene(string sceneName, string name, TankPartType partType) {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).completed +=
                asyncOperation => OnSceneLoaded(asyncOperation, sceneName, name, partType);
        }

        private void OnSceneLoaded(AsyncOperation asyncOperation, string sceneName, string name,
            TankPartType partType) {
            asyncOperation.completed += (op) => { Load(sceneName, name, partType); };
        }

        private void Load(string sceneName, string name, TankPartType partType) {
            OnPreviousSceneLoad?.Invoke();

            Scene currentScene = SceneManager.GetActiveScene();

            foreach (GameObject obj in currentScene.GetRootGameObjects()) {
                obj.SetActive(false);

                if (obj.name == partType.ToString())
                    _partTypeParent = obj.transform;
            }

            _previewScene = currentScene;
            _fileName = name;
            _partType = partType;

            Scene targetScene = SceneManager.GetSceneByName(sceneName);

            SceneManager.SetActiveScene(targetScene);
        }

        #endregion

        #region Load scene

        public void LoadEditScene(string sceneName, GameObject objectToMove, TankPartType partType) {
            // //Destroy event system in previous scene
            // EventSystem existingEventSystem = FindObjectOfType<EventSystem>();
            // if (existingEventSystem != null) {
            //     Debug.Log($"Destroyed {existingEventSystem.name}");
            //     Destroy(existingEventSystem.gameObject);
            // }
            //
            // //Destroy all audio listeners in previous scene
            // AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();
            // if (audioListeners.Length > 0) {
            //     for (int i = 0; i < audioListeners.Length; i++) {
            //         Debug.Log($"Destroyed {audioListeners[i].name}");
            //         Destroy(audioListeners[i].gameObject);
            //     }
            // }

            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).completed +=
                asyncOperation => OnSceneLoaded(asyncOperation, sceneName, objectToMove, partType);
        }

        private void OnSceneLoaded(AsyncOperation asyncOperation, string sceneName, GameObject objectToMove,
            TankPartType partType) {
            asyncOperation.completed += (op) => { Load(sceneName, objectToMove, partType); };
        }

        private void Load(string sceneName, GameObject objectToMove, TankPartType partType) {
            OnPreviousSceneLoad?.Invoke();

            Scene currentScene = SceneManager.GetActiveScene();

            foreach (GameObject obj in currentScene.GetRootGameObjects()) {
                obj.SetActive(false);
            }

            _previewScene = currentScene;
            _partTypeParent = objectToMove.transform.parent;
            _fileName = objectToMove.name;
            _partType = partType;
            objectToMove.transform.parent = null;

            Scene targetScene = SceneManager.GetSceneByName(sceneName);

            SceneManager.MoveGameObjectToScene(objectToMove, targetScene);

            SceneManager.SetActiveScene(targetScene);
        }

        #endregion

        public void ActivatePreviousScene() {
            Scene editorScene = SceneManager.GetActiveScene();
            SceneManager.UnloadSceneAsync(editorScene);

            SceneManager.SetActiveScene(_previewScene);

            foreach (GameObject obj in _previewScene.GetRootGameObjects()) {
                obj.SetActive(true);
            }

            foreach (Transform child in _partTypeParent) {
                child.gameObject.SetActive(false);
            }

            var asyncLoad = FindObjectOfType<PreviewAsyncPreload>();
            asyncLoad.UpdateContent(_partType, _fileName, _partTypeParent);

            OnPreviousSceneLoad?.Invoke();
        }

    }
} //END