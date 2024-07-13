using System.IO;
using Script.Enum;
using Script.Static;
using SimpleFileBrowser;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Manager {
    public class ProjectManager : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public static ProjectManager Instance;

        private const string ControlFile = "controlFile"; //File for UTW project confirmation

        public TankPartType partType = TankPartType.Hull;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }

        #region Create

        public void CreateProject() {
            FileBrowser.ShowSaveDialog((paths) => { OnProjectCreate(paths[0]); },
                () => { Debug.Log("Canceled"); },
                FileBrowser.PickMode.FilesAndFolders, false, null, "New Project", "Create new project", "Create");
        }

        private void OnProjectCreate(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
                Debug.Log($"Created {path}");

                string controlFilePath = Path.Combine(path, ControlFile);
                using (var controlFile = File.CreateText(controlFilePath)) {
                    controlFile.Write(GUID.Generate());
                }

                File.SetAttributes(controlFilePath, FileAttributes.Hidden);
                SceneManager.LoadScene(SceneNames.Editor);
            }
            else {
                Debug.LogError("Project with this name already exists");
            }
        }

        #endregion

        #region Open

        public void OpenProject() {
            FileBrowser.ShowLoadDialog((paths) => { OnProjectOpen(paths[0]); },
                () => { Debug.Log("Canceled"); },
                FileBrowser.PickMode.Folders, false, null, null, "Open project", "Open");
        }

        private void OnProjectOpen(string path) {
            if (File.Exists(Path.Combine(path, ControlFile))) {
                Debug.Log("Selected: " + path);
                partType = TankPartType.Hull; //TODO implement logic
                SceneManager.LoadScene(SceneNames.Editor);
            }
            else {
                Debug.LogError("Cannot open. This is not a UTW project.");
            }
        }

        #endregion

    }
} //END