using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Script.Controller;
using Script.Manager;
using Script.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGLTF;
using Object = UnityEngine.Object;

namespace Script.Component.Parts {
    public class MeshComponent : AComponent {

        public static Action OnMeshChange;

        public GameObject recordPrefab;
        private GameObject _selectedRecord;

        [Header("Mesh Filter")]
        public Mesh defaultMesh;
        public TextMeshProUGUI meshName;

        [Header("Material")]
        public Material defaultMaterial;
        public TextMeshProUGUI materialName;

        /// <summary>
        /// KEY - Button (record) | VALUE - Prefab
        /// </summary>
        private Dictionary<GameObject, Object> _contentData = new();

        protected override void AwakeImpl() {
            ObjectControl.OnObjectDeselected += Deselect;
        }

        protected override void StartImpl() {
            SetMesh(defaultMesh);
            SetMaterial(defaultMaterial);
        }

        protected override void UpdateImpl() {
        }

        #region Mesh

        private void SetMesh(Mesh mesh) {
            ObjectInstance.GetComponent<MeshCollider>().sharedMesh = null;
            ObjectInstance.GetComponent<MeshCollider>().sharedMesh = mesh;
            ObjectInstance.GetComponent<MeshFilter>().sharedMesh = mesh;

            meshName.text = mesh.name;
            OnMeshChange?.Invoke();
        }

        public void OpenMeshPanel() {
            var data = ComponentGridUtils.GetSelectData(ComponentGrid);
            data.SetActive(true);

            ComponentGridUtils.GetName(data).text = "Select Mesh";

            LoadData(data, ProjectManager.MeshFolder, ImportMesh);

            ComponentGridUtils.GetSelectButton(data).onClick.RemoveAllListeners();

            ComponentGridUtils.GetSelectButton(data).onClick.AddListener(SelectMesh);
            ComponentGridUtils.GetCancelButton(data).onClick.AddListener(Cancel);
        }

        private void SelectMesh() {
            var prefab = _contentData[_selectedRecord];
            SetMesh((Mesh)prefab);

            Cancel();
        }

        private async Task<Mesh> ImportMesh(string path) {
            var loader = new GLTFSceneImporter(path, new ImportOptions());
            return await loader.LoadMeshAsync(0, CancellationToken.None);
        }

        #endregion

        #region Material

        private void SetMaterial(Material material) {
            ObjectInstance.GetComponent<MeshRenderer>().material = material;

            materialName.text = material.name;
        }

        public void OpenMaterialPanel() {
            var data = ComponentGridUtils.GetSelectData(ComponentGrid);
            data.SetActive(true);

            ComponentGridUtils.GetName(data).text = "Select Material";

            LoadData(data, ProjectManager.MaterialFolder, ImportMaterial);

            ComponentGridUtils.GetSelectButton(data).onClick.RemoveAllListeners();

            ComponentGridUtils.GetSelectButton(data).onClick.AddListener(SelectMaterial);
            ComponentGridUtils.GetCancelButton(data).onClick.AddListener(Cancel);
        }

        private void SelectMaterial() {
            var material = _contentData[_selectedRecord];
            SetMaterial((Material)material);

            Cancel();
        }

        private async Task<Material> ImportMaterial(string path) {
            var loader = new GLTFSceneImporter(path, new ImportOptions());
            return await loader.LoadMaterialAsync(0);
        }

        #endregion

        private async void LoadData<T>(GameObject data, string folder, Func<string, Task<T>> callback)
            where T : Object {
            var content = ComponentGridUtils.GetContent(data);

            ClearData();

            var resourceFolder = ProjectManager.Instance.GetResourceFolder(folder);

            string[] files = Directory.GetFiles(resourceFolder, "*.gltf");

            foreach (string filePath in files) {
                var fileName = Path.GetFileName(filePath);
                var record = Instantiate(recordPrefab, content.transform);
                record.GetComponentInChildren<TextMeshProUGUI>().text = fileName;
                record.GetComponent<Button>().onClick.AddListener(() => _selectedRecord = record);
                _contentData.Add(record, await callback(filePath));
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

    }
} //END