using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Script.Enum;
using Script.Manager;
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

            yield return new WaitUntil(() => _result != Result.WAIT);

            if (_result.Equals(Result.YES)) {
                if (String.IsNullOrWhiteSpace(newNameInput.text)) {
                    newPanel.SetActive(false);
                    Logger.Instance.LogErrorMessage("Name of the part project cannot be empty!");
                    _result = Result.NO;
                }
                else {
                    LoadScene();
                }
            }

            newPanel.SetActive(false);
        }

        private void LoadScene() {
            SaveManager.Instance.GetCoreData().projectName = newNameInput.text + ProjectUtils.JSON;
            SceneManager.LoadScene(SceneNames.Editor);
        }

        private void CreateContent() {
            ClearData();

            string path = ProjectManager.Instance.GetActiveProjectFolder();
            string[] files = Directory.GetFiles(path, $"*{ProjectUtils.JSON}");

            foreach (var file in files) {
                var go = Instantiate(dataPrefab, _parent);
                go.name = Path.GetFileNameWithoutExtension(file);

                ViewDataUtils.EditButton(go).onClick.AddListener(() => Edit(Path.GetFileName(file)));
                ViewDataUtils.DeleteButton(go).onClick.AddListener(() => StartCoroutine(Delete(file, go)));
                ViewDataUtils.GetName(go).text = Path.GetFileName(file);
                _data.Add(go);
            }
        }

        private void Edit(string fileName) {
            SaveManager.Instance.GetCoreData().projectName = fileName;
            SceneManager.LoadScene(SceneNames.Editor);
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
                Logger.Instance.LogSuccessfulMessage($"File {Path.GetFileName(path)} successfully removed.");
            }
            catch (Exception e) {
                Logger.Instance.LogErrorMessage($"File {Path.GetFileName(path)} cannot be removed.");
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