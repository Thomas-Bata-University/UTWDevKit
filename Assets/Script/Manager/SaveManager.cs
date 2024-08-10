using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Script.Controller;
using Script.Enum;
using Script.Mesh;
using Script.SO;
using Script.Utils;
using UnityEngine;
using UnityEngine.Events;
using Logger = Script.Log.Logger;

namespace Script.Manager {
    public class SaveManager : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public static SaveManager Instance;
        public static UnityAction<string, Transform> OnMeshLoad;

        public GameObject objectPrefab;

        /// <summary>
        /// KEY - File name without extension | VALUE - (KEY - Selectable | VALUE - Data about object)
        /// </summary>
        public Dictionary<string, Dictionary<Transform, DataToSave>> Data = new();
        private DataList _dataList = new();

        [Serializable]
        public struct Parts {

            public TankPartType partType;
            public ComponentSO part;

        }

        public Parts[] componentParts;

        public UnityEngine.Mesh defaultMesh; //If we dont find mesh or doesnt exists, use default

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }

        public void Save() {
            if (!Data.ContainsKey(_dataList.projectName)) {
                Debug.Log("INFO - Cannot save, there are no data");
                return;
            }

            string path = ProjectManager.Instance.GetActiveProjectFolder();

            _dataList.items = new List<DataToSave>();

            foreach (var dataItem in Data[_dataList.projectName].Values) {
                DataToSave saveData = MapData(dataItem);

                _dataList.items.Add(saveData);
            }

            string json = JsonUtility.ToJson(_dataList, true);
            File.WriteAllText(Path.Combine(path, _dataList.fileName), json);

            Logger.Instance.LogMessage("Saved");
        }

        public void Load() {
            if (!Data.ContainsKey(_dataList.projectName)) {
                Debug.Log("INFO - Cannot load, there are no data");
                return;
            }

            string path = ProjectManager.Instance.GetActiveProjectFolder();
            string fullPath = Path.Combine(path, _dataList.fileName);
            if (File.Exists(fullPath)) {
                string json = File.ReadAllText(fullPath);
                DataList loadedData = JsonUtility.FromJson<DataList>(json);

                SetIds(loadedData);
                SetCamera(loadedData);
                SetTankPartObject();
            }
        }

        public async Task<Transform> Preload(string fullPath, TankPartType partType, Transform partTypeParent) {
            if (File.Exists(fullPath)) {
                string json = await File.ReadAllTextAsync(fullPath);
                DataList loadedData = JsonUtility.FromJson<DataList>(json);

                var parent = new GameObject(Path.GetFileNameWithoutExtension(fullPath)).transform;
                parent.parent = partTypeParent;
                parent.gameObject.SetActive(false);

                foreach (var data in loadedData.items) {
                    var go = Instantiate(objectPrefab, parent);
                    go.name = data.objectName;

                    var selectable = ObjectUtils.GetReference(go.transform);
                    selectable.position = data.position;
                    selectable.rotation = Quaternion.Euler(data.rotation);

                    ObjectUtils.GetTag(go.transform).tag = data.Tag;
                    if (await GltfManager.Instance.OnMeshLoad(data.pathToGraphic, selectable)) {
                        selectable.GetComponent<CombineMeshes>().Merge();
                    }
                    else {
                        selectable.GetComponent<MeshFilter>().mesh = defaultMesh;
                    }

                    GetData(Path.GetFileNameWithoutExtension(fullPath)).Add(selectable, MapData(data));

                    Debug.Log($"Preloaded - {partType} - {Path.GetFileNameWithoutExtension(fullPath)} - {go.name}");
                }

                return parent;
            }

            return null;
        }


        private void SetIds(DataList loadedData) {
            foreach (var id in loadedData.ids) {
                UniqueIdGenerator.Instance.SetCurrentID(id);
            }
        }

        private void SetCamera(DataList loadedData) {
            var cameraControl = FindObjectOfType<CameraControl>();
            cameraControl.transform.position = loadedData.cameraPos;
            cameraControl.transform.rotation = Quaternion.Euler(loadedData.cameraRot);
            cameraControl.orientation.rotation = Quaternion.Inverse(cameraControl.transform.rotation);
        }

        private void SetTankPartObject() {
            var componentControl = FindObjectOfType<ComponentControl>();

            foreach (var kvp in Data[_dataList.projectName]) {
                ProjectUtils.CreateOutline(kvp.Key.gameObject);

                ComponentSO component = GetComponents(kvp.Value.type);
                componentControl.CreateGridForObject(kvp.Key.parent, component.Initialize());
                kvp.Key.parent.parent.gameObject.SetActive(true);

                OnMeshLoad?.Invoke(kvp.Value.pathToGraphic, kvp.Key);
            }
        }

        private ComponentSO GetComponents(TankPartType partType) {
            foreach (var entry in componentParts) {
                if (entry.partType == partType) {
                    return entry.part;
                }
            }

            return null;
        }

        public DataList GetCoreData() {
            return _dataList;
        }

        public Dictionary<Transform, DataToSave> GetData(string key) {
            if (Data.TryGetValue(key, out var data)) return data;
            var newData = new Dictionary<Transform, DataToSave>();
            Data.Add(key, newData);
            return newData;
        }

        public DataToSave GetData(Transform key) {
            var data1 = GetData(_dataList.projectName);

            if (data1.TryGetValue(key, out var data)) return data;
            var newData = new DataToSave();
            data1.Add(key, newData);
            return newData;
        }

        public void Remove(Transform key) {
            Debug.Log($"Object '{key.name}' deleted from data to save");
            Data[_dataList.projectName].Remove(key);
        }

        private DataToSave MapData(DataToSave data) {
            return new DataToSave() {
                objectName = data.objectName,
                type = data.type,
                position = data.position,
                rotation = data.rotation,
                pathToGraphic = data.pathToGraphic,
                Tag = data.Tag
            };
        }

        [Serializable]
        public class DataList {

            //Core data
            public string projectName; //Not name of the whole project
            public string fileName;
            public List<IDs> ids = new();

            //Camera
            public Vector3 cameraPos;
            public Vector3 cameraRot;

            //Tank part objects
            public List<DataToSave> items = new();

        }

        [Serializable]
        public class DataToSave {

            public string objectName;
            public TankPartType type;
            public Vector3 position;
            public Vector3 rotation;
            public string pathToGraphic;
            public string Tag;

        }

        [Serializable]
        public class IDs {

            public TankPartType type;
            public int count;

        }

    }
} //END