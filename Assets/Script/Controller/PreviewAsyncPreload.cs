using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Script.Enum;
using Script.Manager;
using Script.Other;
using Script.SceneLoad;
using Script.Static;
using Script.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Logger = Script.Log.Logger;
using Async = System.Threading.Tasks;

namespace Script.Controller {
    public class PreviewAsyncPreload : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        private enum Result {

            YES,
            NO,
            WAIT

        }

        /// <summary>
        /// KEY - Part type | VALUE - Parent - Button
        /// </summary>
        private Dictionary<TankPartType, Dictionary<Transform, GameObject>> _data = new();

        /// <summary>
        /// KEY - Imported object parent | VALUE - Array of selectable object transform
        /// </summary>
        private Dictionary<Transform, Transform[]> _bounds = new();

        public GameObject dataPrefab;
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

        [Header("Data")]
        public TMP_InputField filter;
        public TextMeshProUGUI countText;
        private int _count;

        private int _activeTab;
        private TankPartType _partType;

        private async void Start() {
            SwitchTab((int)TankPartType.HULL);
            createNewButton.onClick.AddListener((() => StartCoroutine(CreateNewPart())));
            filter.onValueChanged.AddListener(OnInputChanged);

            Debug.Log("Preload start");
            await PreloadData();
            Debug.Log("Preload done");

            await WaitForFewSeconds(1); //Wait for few seconds to show player state
            LoadingScreen.Instance.Hide();

            OnInputChanged(filter.text);
        }

        private async Async.Task PreloadData() {
            SaveManager.Instance.Data.Clear();

            var hull = CreateContent(TankPartType.HULL);
            var turret = CreateContent(TankPartType.TURRET);
            var suspension = CreateContent(TankPartType.SUSPENSION);
            var weaponry = CreateContent(TankPartType.WEAPONRY);

            await Async.Task.WhenAll(hull, turret, suspension, weaponry);
        }

        public void SwitchTab(int index) {
            cameraBounds.info.SetActive(false);

            foreach (var tab in tabs) {
                tab.SetActive(false);
            }

            tabs[index].SetActive(true);
            _activeTab = index;

            foreach (var button in buttons) {
                button.color = inactive;
            }

            buttons[index].color = active;
            _partType = (TankPartType)_activeTab;
            createButtonText.text = "Create new " + _partType;

            ProjectManager.Instance.partType = _partType;
            OnInputChanged(filter.text);
        }

        private void LoadScene() {
            SaveManager.Instance.GetCoreData().projectName = newNameInput.text;
            SaveManager.Instance.GetCoreData().fileName = newNameInput.text + ProjectUtils.JSON;
            SceneLoadManager.Instance.LoadNewScene(SceneNames.Editor, _partType);
        }

        private async Async.Task CreateContent(TankPartType partType) {
            var partTypeParent = new GameObject(partType.ToString()).transform;

            string[] files = GetFiles(partType);

            if (files.Length == 0) {
                UpdateText(partType, $"{partType} - No content found");
                return;
            }

            int count = 0;
            UpdateCount(partType, count, files.Length);

            if (_data.ContainsKey(partType)) return;

            foreach (var file in files) {
                await CreateNewContent(partType, file, partTypeParent);

                UpdateCount(partType, ++count, files.Length);
            }

            UpdateText(partType, $"{partType} - DONE ({files.Length})");
        }

        public async void UpdateContent(TankPartType partType, string fileName, Transform partTypeParent) {
            SaveManager.Instance.GetData(fileName).Clear();

            string file = Path.Combine(ProjectManager.Instance.GetActiveProjectFolder(partType),
                fileName + ProjectUtils.JSON);

            await CreateNewContent(partType, file, partTypeParent);
        }

        private async Async.Task CreateNewContent(TankPartType partType, string file, Transform partTypeParent) {
            var go = Instantiate(dataPrefab, GetParent(partType));
            go.name = Path.GetFileNameWithoutExtension(file);
            ViewDataUtils.GetName(go).text = Path.GetFileNameWithoutExtension(file);

            var parent = await SaveManager.Instance.Preload(file, partType, partTypeParent);

            ViewDataUtils.ViewButton(go).onClick.AddListener(() => View(parent));
            ViewDataUtils.EditButton(go).onClick.AddListener(() => Edit(file, go, parent, partType));
            ViewDataUtils.DeleteButton(go).onClick
                .AddListener(() => StartCoroutine(Delete(file, go, partType, parent)));
            GetData(partType).Add(parent, go);
            _bounds.Add(parent,
                parent.Cast<Transform>().Select(ObjectUtils.GetReference).ToArray());
        }

        private string[] GetFiles(TankPartType partType) {
            return Directory.GetFiles(ProjectManager.Instance.GetActiveProjectFolder(partType),
                $"*{ProjectUtils.JSON}");
        }

        private string[] GetFiles() {
            return Directory.GetFiles(ProjectManager.Instance.GetActiveProjectFolder(), $"*{ProjectUtils.JSON}");
        }

        private Transform GetParent(TankPartType partType) {
            return tabs[(int)partType].transform.GetChild(0).GetChild(0); //Get content
        }

        private void UpdateCount(TankPartType partType, int actualCount, int expectedCount) {
            LoadingScreen.Instance.SetText($"Loading {partType} - {actualCount}/{expectedCount}", partType);
        }

        private void UpdateText(TankPartType partType, string text) {
            LoadingScreen.Instance.SetText(text, partType);
        }

        private Dictionary<Transform, GameObject> GetData(TankPartType key) {
            if (_data.TryGetValue(key, out var data)) return data;
            var newData = new Dictionary<Transform, GameObject>();
            _data.Add(key, newData);
            return newData;
        }

        public void GoBack() {
            SceneManager.LoadScene(SceneNames.MainMenu);
        }

        private async Async.Task WaitForFewSeconds(int seconds = 2) {
            await Async.Task.Delay(seconds * 1000);
        }

        private void OnInputChanged(string input) {
            _count = 0;
            if (!_data.ContainsKey(_partType)) {
                SetCount();
                return;
            }

            string lowerInput = input.ToLower();
            foreach (var kvp in _data[_partType]) {
                if (kvp.Key.name.ToLower().Contains(lowerInput)) {
                    kvp.Value.SetActive(true);
                    _count++;
                }
                else {
                    kvp.Value.SetActive(false);
                }
            }

            SetCount();
        }

        private void SetCount() {
            countText.text = $"Count: {_count}";
        }

        #region Button actions

        private IEnumerator CreateNewPart() {
            _result = Result.WAIT;
            newPanel.SetActive(true);
            newNameInput.text = null;

            yield return new WaitUntil(() => _result != Result.WAIT);

            if (_result.Equals(Result.YES)) {
                if (Validate())
                    LoadScene();
            }

            newPanel.SetActive(false);
        }

        private void View(Transform parent) {
            cameraBounds.Restart();

            foreach (var p in _bounds.Keys) {
                p.gameObject.SetActive(false);
            }

            var bounds = cameraBounds.GetTargetsBounds(_bounds[parent]);
            cameraBounds.SetCamera(bounds);
            parent.gameObject.SetActive(true);
        }

        private void Edit(string path, GameObject go, Transform parent, TankPartType partType) {
            RemoveData(go, partType, parent);
            _bounds.Remove(parent);
            SaveManager.Instance.GetCoreData().projectName = Path.GetFileNameWithoutExtension(path);
            SaveManager.Instance.GetCoreData().fileName = Path.GetFileName(path);
            SceneLoadManager.Instance.LoadEditScene(SceneNames.Editor, parent.gameObject, partType);
        }

        private IEnumerator Delete(string path, GameObject go, TankPartType partType, Transform parent) {
            _result = Result.WAIT;
            deletePanel.SetActive(true);
            deleteInfo.text = $"Are you sure you want to remove {Path.GetFileName(path)}?";

            yield return new WaitUntil(() => _result != Result.WAIT);

            if (_result.Equals(Result.YES)) {
                DeleteFile(path, go, partType, parent);
            }

            deletePanel.SetActive(false);
            cameraBounds.Restart();
            OnInputChanged(countText.text);
        }

        public void Click(int index) {
            _result = (Result)index;
        }

        private void RemoveData(GameObject go, TankPartType partType, Transform parent) {
            _data[partType].Remove(parent);
            Destroy(go);
        }

        private void DeleteFile(string path, GameObject go, TankPartType partType, Transform parent) {
            try {
                RemoveData(go, partType, parent);
                File.Delete(path);
                Logger.Instance.LogSuccessfulMessage(
                    $"Project '{Path.GetFileNameWithoutExtension(path)}' successfully deleted.");
            }
            catch (Exception e) {
                Logger.Instance.LogErrorMessage(
                    $"Project '{Path.GetFileNameWithoutExtension(path)}' cannot be deleted.");
            }
        }

        #endregion

        #region Validate

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

        #endregion

    }
} //END