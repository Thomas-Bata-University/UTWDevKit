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

        /// <summary>
        /// KEY - Button (record) | VALUE - Data about imported .gltf object
        /// </summary>
        private Dictionary<GameObject, Data> _contentData = new();

        protected override void AwakeImpl() {
            ObjectControl.OnObjectDeselected += Deselect;
        }

        protected override void StartImpl() {
        }

        protected override void UpdateImpl() {
        }

        public void OpenMeshPanel() {
            var data = ComponentGridUtils.GetSelectData(ComponentGrid);
            data.SetActive(true);

            ComponentGridUtils.GetName(data).text = "Select Mesh";

            LoadData(data, ProjectManager.GraphicFolder, ImportMesh);

            ComponentGridUtils.GetSelectButton(data).onClick.RemoveAllListeners();

            ComponentGridUtils.GetSelectButton(data).onClick.AddListener(SelectMesh);
            ComponentGridUtils.GetCancelButton(data).onClick.AddListener(Cancel);
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

            Cancel();
        }

        private async Task<Data> ImportMesh(string path) {
            try {
                var gltf = new GltfImport();
                var settings = new ImportSettings {
                    GenerateMipMaps = true,
                    AnisotropicFilterLevel = 3,
                    NodeNameMethod = NameImportMethod.OriginalUnique
                };

                var success = await gltf.Load(path, settings);

                if (success) {
                    var instantiator = new GameObjectInstantiator(gltf, ObjectInstance);
                    return new Data(gltf, ObjectInstance, instantiator, Path.GetFileName(path));
                }

                throw new Exception($"An error occured during importing GLTF file: {path}");
            }
            catch (Exception e) {
                Logger.Instance.LogErrorMessage(e.Message, 5f);
                return null;
            }
        }

        private async void LoadData(GameObject data, string folder, Func<string, Task<Data>> callback) {
            var content = ComponentGridUtils.GetContent(data);

            ClearData();

            var resourceFolder = ProjectManager.Instance.GetResourceFolder(folder);

            string[] files = Directory.GetFiles(resourceFolder, "*.gltf");

            foreach (string filePath in files) {
                var import = await callback(filePath);
                if (import is null) continue;

                var fileName = Path.GetFileName(filePath);
                var record = Instantiate(recordPrefab, content.transform);
                record.GetComponentInChildren<TextMeshProUGUI>().text = fileName;
                record.GetComponent<Button>().onClick.AddListener(() => _selectedRecord = record);
                _contentData.Add(record, import);
            }
        }

        private void ClearData() {
            foreach (var data in _contentData) {
                Destroy(data.Key);
            }

            _contentData.Clear();
        }

        private void Cancel() {
            var data = ComponentGridUtils.GetSelectData(ComponentGrid);
            data.SetActive(false);
        }

        private void Deselect(Transform deselectedObject) {
            if (deselectedObject == ObjectInstance) {
                Cancel();
            }
        }

        private class Data {

            public GltfImport Gltf { get; set; }
            public Transform ParentObject { get; set; }
            public GameObjectInstantiator Instantiator { get; set; }
            public string FileName { get; set; }

            public Data(GltfImport gltf, Transform parentObject, GameObjectInstantiator instantiator, string fileName) {
                Gltf = gltf;
                ParentObject = parentObject;
                Instantiator = instantiator;
                FileName = fileName;
            }

        }

    }
} //END