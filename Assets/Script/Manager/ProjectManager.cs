using System;
using System.IO;
using Script.Enum;
using Script.Static;
using SimpleFileBrowser;
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

        private const string HullFolder = "Hull";
        private const string TurretFolder = "Turret";
        private const string SuspensionFolder = "Suspension";
        private const string WeaponryFolder = "Weaponry";
        private const string ResourceFolder = "Resources";

        //Resources
        public const string GraphicFolder = "Graphic";

        public TankPartType partType = TankPartType.Hull;

        private string _projectPath;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }

            Debug.developerConsoleVisible = true;
            Debug.developerConsoleEnabled = true;
        }

        #region Create project

        public void CreateProject() {
            FileBrowser.ShowSaveDialog((paths) => { OnProjectCreate(paths[0]); },
                () => { Debug.Log("Canceled"); },
                FileBrowser.PickMode.FilesAndFolders, false, null, "New Project", "Create new project", "Create");
        }

        private void OnProjectCreate(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
                Debug.Log($"Created {path}");
                _projectPath = path;

                CreateFolders(path);

                SceneManager.LoadScene(SceneNames.Preview);
            }
            else {
                Debug.LogError("Project with this name already exists");
            }
        }

        private void CreateFolders(string path) {
            string controlFilePath = Path.Combine(path, ControlFile);
            using (var controlFile = File.CreateText(controlFilePath)) {
                controlFile.Write(Guid.NewGuid());
            }

            File.SetAttributes(controlFilePath, FileAttributes.Hidden);

            CreateDirectory(path, HullFolder);
            CreateDirectory(path, TurretFolder);
            CreateDirectory(path, SuspensionFolder);
            CreateDirectory(path, WeaponryFolder);
            CreateDirectory(path, ResourceFolder);

            CreateDirectory(path, Path.Combine(ResourceFolder, GraphicFolder));
        }

        private void CreateDirectory(string path, string folderName) {
            Directory.CreateDirectory(Path.Combine(path, folderName));
        }

        #endregion

        #region Open project

        public void OpenProject() {
            FileBrowser.ShowLoadDialog((paths) => { OnProjectOpen(paths[0]); },
                () => { Debug.Log("Canceled"); },
                FileBrowser.PickMode.Folders, false, null, null, "Open project", "Open");
        }

        private void OnProjectOpen(string path) {
            if (File.Exists(Path.Combine(path, ControlFile))) {
                Debug.Log("Selected: " + path);
                _projectPath = path;

                SceneManager.LoadScene(SceneNames.Preview);
            }
            else {
                Debug.LogError("Cannot open. This is not a UTW project.");
            }
        }

        #endregion

        public string GetResourceFolder(string folder) {
            return Path.Combine(_projectPath, ResourceFolder, folder);
        }

    }
} //END