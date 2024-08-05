using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GLTFast;
using Script.Controller;
using Script.Manager;
using Script.Mesh;
using Script.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = Script.Log.Logger;

namespace Script.Component.Parts {
    public class GraphicComponent : AComponent {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public static Action OnMeshChange;

        public GameObject recordPrefab;
        private GameObject _selectedRecord;

        [Header("Mesh Filter")]
        public TextMeshProUGUI meshName;

        [Header("Select data hierarchy")]
        private GameObject _data;
        private Button _selectButton;
        private Button _cancelButton;
        private TMP_InputField _filterInput;

        [Header("Metadata")]
        public TextMeshProUGUI filePath;
        public TextMeshProUGUI fileExtension;
        public TextMeshProUGUI fileSize;
        public TextMeshProUGUI fileLastChange;

        /// <summary>
        /// KEY - Button (record) | VALUE - Data about imported .gltf object
        /// </summary>
        private Dictionary<GameObject, Data> _contentData = new();

        protected override void AwakeImpl() {
            ObjectControl.OnObjectDeselected += Deselect;
            SaveManager.OnMeshLoad += OnMeshLoad;
        }

        protected override void StartImpl() {
            _data = ComponentGridUtils.GetSelectData(ComponentGrid);
            _selectButton = ComponentGridUtils.GetSelectButton(_data);
            _cancelButton = ComponentGridUtils.GetCancelButton(_data);
            _filterInput = ComponentGridUtils.GetFilterInput(_data);
        }

        protected override void UpdateImpl() {
        }

        #region Filter

        private void OnInputChanged(string input) {
            string lowerInput = input.ToLower();
            foreach (var kvp in _contentData) {
                kvp.Key.SetActive(kvp.Value.FileName.ToLower().Contains(lowerInput));
            }
        }

        #endregion

        public void OpenMeshPanel() {
            _selectButton.interactable = false;
            _data.SetActive(true);

            ComponentGridUtils.GetName(_data).text = "Select Mesh";

            LoadData(ProjectManager.GraphicFolder, ImportMesh);

            _selectButton.onClick.AddListener(SelectMesh);
            _cancelButton.onClick.AddListener(Cancel);
            _filterInput.onValueChanged.AddListener(OnInputChanged);
        }

        private async void SelectMesh() {
            var data = _contentData[_selectedRecord];

            var parent = data.ParentObject;
            if (parent.childCount > 0 && parent.GetChild(0) is not null)
                Destroy(parent.GetChild(0).gameObject);

            await data.Gltf.InstantiateMainSceneAsync(data.Instantiator);

            meshName.text = data.FileName;

            parent.name = "Graphic";
            parent.GetComponent<CombineMeshes>().Merge();

            OnMeshChange?.Invoke();

            SetMetadata(data.FilePath);

            Cancel();

            SaveManager.Instance.GetData(ObjectInstance).pathToGraphic = data.FilePath;
        }

        private void OnMeshLoad(string path, Transform objectInstance) {
            OnMeshChange?.Invoke(); //Call to set canvas middle

            if (String.IsNullOrEmpty(path) || ObjectInstance != objectInstance) {
                return;
            }

            meshName.text = Path.GetFileNameWithoutExtension(path);
            SetMetadata(path);
        }

        private async Task<Data> ImportMesh(string path) {
            try {
                return await GltfManager.Instance.ImportMesh(path, ObjectInstance);
            }
            catch (Exception e) {
                Logger.Instance.LogErrorMessage(e.Message, 5f);
                return null;
            }
        }

        private async void LoadData(string folder, Func<string, Task<Data>> callback) {
            var content = ComponentGridUtils.GetContent(_data);

            ClearData();

            var resourceFolder = ProjectManager.Instance.GetResourceFolder(folder);

            string[] files = Directory.GetFiles(resourceFolder, $"*{ProjectUtils.GLTF}");

            foreach (string filePath in files) {
                var import = await callback(filePath);
                if (import is null) continue;

                var fileName = Path.GetFileName(filePath);
                var record = Instantiate(recordPrefab, content.transform);
                record.GetComponentInChildren<TextMeshProUGUI>().text = fileName;
                record.GetComponent<Button>().onClick.AddListener(() => SelectRecord(record));
                _contentData.Add(record, import);
            }
        }

        private void SelectRecord(GameObject record) {
            _selectedRecord = record;
            _selectButton.interactable = true;
        }

        private void ClearData() {
            foreach (var data in _contentData) {
                Destroy(data.Key);
            }

            _contentData.Clear();
        }

        private void SetMetadata(string filePath) {
            FileInfo fileInfo = new FileInfo(filePath);
            this.filePath.text = "Path: " + filePath;
            fileExtension.text = "Extension: " + Path.GetExtension(filePath);
            fileSize.text = "Size: " + ByteUtils.ToMB(fileInfo.Length) + " MB";
            fileLastChange.text = "Last change: " + fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void Cancel() {
            var data = ComponentGridUtils.GetSelectData(ComponentGrid);
            data.SetActive(false);

            _selectButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();
            _filterInput.onValueChanged.RemoveAllListeners();
        }

        private void Deselect(Transform deselectedObject) {
            if (deselectedObject == ObjectInstance) {
                Cancel();
            }
        }

        public class Data {

            public GltfImport Gltf { get; set; }
            public Transform ParentObject { get; set; }
            public GameObjectInstantiator Instantiator { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }

            public Data(GltfImport gltf, Transform parentObject, GameObjectInstantiator instantiator, string fileName,
                string filePath) {
                Gltf = gltf;
                ParentObject = parentObject;
                Instantiator = instantiator;
                FileName = fileName;
                FilePath = filePath;
            }

        }

        private void OnDestroy() {
            ObjectControl.OnObjectDeselected -= Deselect;
            SaveManager.OnMeshLoad -= OnMeshLoad;
        }

    }
} //END