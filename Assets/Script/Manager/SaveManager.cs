using System;
using System.Collections.Generic;
using System.IO;
using Script.Controller;
using Script.Enum;
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

        private Dictionary<Transform, DataToSave> _data = new();
        private DataList _dataList = new();

        [Serializable]
        public struct Parts {

            public TankPartType partType;
            public ComponentSO part;

        }

        public Parts[] componentParts;

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
            string path = ProjectManager.Instance.GetActiveProjectFolder();

            _dataList.items = new List<DataToSave>();

            foreach (var dataItem in _data.Values) {
                DataToSave saveData = MapData(dataItem);

                _dataList.items.Add(saveData);
            }

            string json = JsonUtility.ToJson(_dataList, true);
            File.WriteAllText(Path.Combine(path, _dataList.projectName), json);

            Logger.Instance.LogMessage("Saved");
        }

        public void Load() {
            _data.Clear();
            string path = ProjectManager.Instance.GetActiveProjectFolder();
            string fullPath = Path.Combine(path, _dataList.projectName);
            if (File.Exists(fullPath)) {
                string json = File.ReadAllText(fullPath);
                DataList loadedData = JsonUtility.FromJson<DataList>(json);

                SetCamera(loadedData);
                SetTankPartObject(loadedData);
            }
        }

        private void SetCamera(DataList loadedData) {
            var cameraControl = FindObjectOfType<CameraControl>().transform;
            cameraControl.position = loadedData.cameraPos;
            cameraControl.rotation = Quaternion.Euler(loadedData.cameraRot);
        }

        private void SetTankPartObject(DataList loadedData) {
            var componentControl = FindObjectOfType<ComponentControl>();

            foreach (var data in loadedData.items) {
                var go = Instantiate(objectPrefab);

                var selectable = ObjectUtils.GetReference(go.transform);
                selectable.position = data.position;
                selectable.rotation = Quaternion.Euler(data.rotation);

                ObjectUtils.GetTag(go.transform).tag = data.Tag;

                ComponentSO component = GetComponents(data.type);
                componentControl.CreateGridForObject(go.transform, component.Initialize());
                OnMeshLoad?.Invoke(data.pathToGraphic, selectable);
                _data.Add(selectable, MapData(data));
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

        public DataToSave GetData(Transform key) {
            if (_data.TryGetValue(key, out var data)) return data;
            var newData = new DataToSave();
            _data.Add(key, newData);
            return newData;
        }

        public void Remove(Transform key) {
            Debug.Log($"Object {key.name} deleted from data to save");
            _data.Remove(key);
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

            //Main data
            public string projectName; //Not name of the whole project

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

    }
} //END