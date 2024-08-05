using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Script.Enum;
using Script.Manager;
using Script.Other;
using Script.Static;
using Script.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Logger = Script.Log.Logger;

namespace Script.Controller {
    public class TabControl : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        private enum Result {

            YES,
            NO,
            WAIT

        }

        public GameObject dataPrefab;
        private Transform _parent;
        private List<GameObject> _data = new();
        private Dictionary<Transform, Transform[]> _bounds = new();
        public BoundCamera cameraBounds;

        public GameObject[] tabs;
        public Image[] buttons;

        public Color active, inactive;
        public TextMeshProUGUI createButtonText;
        private Result _result = Result.WAIT;

        [Header("Delete")]
        public TextMeshProUGUI deleteInfo;
        public GameObject deletePanel;

        [Header("Create new")]
        public Button createNewButton;
        public TMP_InputField newNameInput;
        public GameObject newPanel;
        private int _minProjectName = 4;

        private int _activeTab;

        private void Start() {
            SwitchTab((int)TankPartType.Hull);
            createNewButton.onClick.AddListener((() => StartCoroutine(CreateNewPart())));
        }

        public void SwitchTab(int index) {
            foreach (var tab in tabs) {
                tab.SetActive(false);
            }

            tabs[index].SetActive(true);
            _activeTab = index;

            foreach (var button in buttons) {
                button.color = inactive;
            }

            buttons[index].color = active;
            createButtonText.text = "Create new " + ((TankPartType)_activeTab);

            _parent = tabs[index].transform.GetChild(0).GetChild(0); //Get content

            ProjectManager.Instance.partType = (TankPartType)_activeTab;

            CreateContent();
        }

        public IEnumerator CreateNewPart() {
            _result = Result.WAIT;
            newPanel.SetActive(true);
            newNameInput.text = null;

            yield return new WaitUntil(() => _result != Result.WAIT);

            if (_result.Equals(Result.YES)) {
                if (Validate())
                    LoadScene();
            }
        }

        private bool Validate() {
            if (String.IsNullOrWhiteSpace(newNameInput.text)) {
                Logger.Instance.LogErrorMessage("Name of the part project cannot be empty!");
            }
            else if (CheckAlreadyExists(newNameInput.text)) {
                Logger.Instance.LogErrorMessage("Project with this name already exists!");
            }
            else if (newNameInput.text.Length < _minProjectName) {
                Logger.Instance.LogErrorMessage($"Project name cannot be shorter than {_minProjectName}!");
            }
            else {
                return true;
            }

            newPanel.SetActive(false);
            _result = Result.NO;
            return false;
        }

        private bool CheckAlreadyExists(string projectName) {
            string[] files = GetFiles();

            foreach (var file in files) {
                if (projectName.Equals(Path.GetFileNameWithoutExtension(file))) {
                    return true;
                }
            }

            return false;
        }

        private void LoadScene() {
            SaveManager.Instance.GetCoreData().projectName = newNameInput.text;
            SaveManager.Instance.GetCoreData().fileName = newNameInput.text + ProjectUtils.JSON;
            SceneManager.LoadScene(SceneNames.Editor);
        }

        private async void CreateContent() {
            ClearData();

            string[] files = GetFiles();

            SaveManager.Instance.Data.Clear();

            foreach (var file in files) {
                var go = Instantiate(dataPrefab, _parent);
                go.name = Path.GetFileNameWithoutExtension(file);

                var parent = await SaveManager.Instance.Preload(file);

                ViewDataUtils.ViewButton(go).onClick.AddListener(() => View(parent));
                ViewDataUtils.EditButton(go).onClick.AddListener(() => Edit(Path.GetFileName(file), parent));
                ViewDataUtils.DeleteButton(go).onClick.AddListener(() => StartCoroutine(Delete(file, go)));
                ViewDataUtils.GetName(go).text = Path.GetFileNameWithoutExtension(file);
                _data.Add(go);
                _bounds.Add(parent,
                    parent.Cast<Transform>().Select(ObjectUtils.GetReference).ToArray());
            }
        }

        private string[] GetFiles() {
            return Directory.GetFiles(ProjectManager.Instance.GetActiveProjectFolder(), $"*{ProjectUtils.JSON}");
        }

        private void View(Transform parent) {
            foreach (var p in _bounds.Keys) {
                p.gameObject.SetActive(false);
            }

            parent.gameObject.SetActive(true);
            var bounds = cameraBounds.GetTargetsBounds(_bounds[parent]);
            cameraBounds.SetCamera(bounds);
        }

        private void Edit(string fileName, Transform parent) {
            SaveManager.Instance.GetCoreData().projectName = Path.GetFileNameWithoutExtension(fileName);
            SaveManager.Instance.GetCoreData().fileName = fileName;
            ProjectManager.Instance.MoveObjectToScene(SceneNames.Editor, parent.gameObject);
        }

        private IEnumerator Delete(string path, GameObject go) {
            _result = Result.WAIT;
            deletePanel.SetActive(true);
            deleteInfo.text = $"Are you sure you want to remove {Path.GetFileName(path)}?";

            yield return new WaitUntil(() => _result != Result.WAIT);

            if (_result.Equals(Result.YES)) {
                DeleteFile(path, go);
            }

            deletePanel.SetActive(false);
        }

        public void Click(int index) {
            _result = (Result)index;
        }

        private void DeleteFile(string path, GameObject go) {
            try {
                _data.Remove(go);
                Destroy(go);
                File.Delete(path);
                Logger.Instance.LogSuccessfulMessage($"Project '{Path.GetFileNameWithoutExtension(path)}' successfully deleted.");
            }
            catch (Exception e) {
                Logger.Instance.LogErrorMessage($"Project '{Path.GetFileNameWithoutExtension(path)}' cannot be deleted.");
                Debug.LogError(e);
            }
        }

        private void ClearData() {
            foreach (var data in _data) {
                Destroy(data);
            }

            _data.Clear();
        }

        public void GoBack() {
            SceneManager.LoadScene(SceneNames.MainMenu);
        }

    }
} //END