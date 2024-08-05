using System.Collections.Generic;
using System.Linq;
using Script.Controller;
using Script.Enum;
using Script.Manager;
using Script.SO;
using Script.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Action = System.Action;
using Logger = Script.Log.Logger;

namespace Script.Task {
    /// <summary>
    /// DO NOT USE UNITY LIFECYCLE METHODS, instead use Impl methods.
    /// </summary>
    public abstract class ATask : MonoBehaviour {

        public static UnityAction<Transform, Dictionary<ComponentType, GameObject>> OnPartCreation;

        public string description;
        private bool _isComplete;

        public TextMeshProUGUI text;
        public Image isCompleteImage;

        public ComponentSO components;
        public GameObject prefab;
        protected string Tag;

        public TextMeshProUGUI countText;
        protected int ActualCount, MaxCount = 1;

        public TankPartType partType;

        private void Awake() {
            ObjectControl.OnObjectRemove += RemoveObject;
            AwakeImpl();
        }

        private void Start() {
            text.text = description;
            isCompleteImage.color = _isComplete ? Color.green : Color.red;
            ActualCount = GameObject.FindGameObjectsWithTag(Tag).Length;
            SetText(ActualCount);

            StartImpl();
        }

        private void Update() {
            if (GameObject.FindGameObjectsWithTag(Tag).Length >= MaxCount) {
                TaskComplete();
            }
            else {
                TaskReset();
            }

            UpdateImpl();
        }

        protected abstract void AwakeImpl();

        protected abstract void StartImpl();

        protected abstract void UpdateImpl();

        public void TaskComplete() {
            _isComplete = true;
            SetColor();
        }

        public void TaskReset() {
            _isComplete = false;
            SetColor();
        }

        public virtual void Create() {
            var go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            OnPartCreation?.Invoke(go.transform, components.Initialize());
            SetText(++ActualCount);
            int id = UniqueIdGenerator.Instance.GetNextID(partType);

            go.name = $"{prefab.name}_{id}";

            var selectable = ObjectUtils.GetReference(go.transform);
            ProjectUtils.CreateOutline(selectable.gameObject);

            var saveData = SaveManager.Instance.GetData(selectable);
            saveData.type = partType;
            saveData.Tag = Tag;
            saveData.objectName = go.name;

            var coreData = SaveManager.Instance.GetCoreData();
            try {
                var getId = coreData.ids.First(ds => ds.type.Equals(partType));
                getId.count = ++id;
            }
            catch {
                coreData.ids.Add(new SaveManager.IDs() { type = partType, count = ++id });
            }
        }

        private void RemoveObject(Transform selectedObject) {
            if (ObjectUtils.GetType(selectedObject).Equals(Tag)) {
                SetText(--ActualCount);
            }
        }

        private void SetText(int count) {
            countText.text = count + "/" + MaxCount;
        }

        private void SetColor() {
            isCompleteImage.color = _isComplete ? Color.green : Color.red;
        }

        protected void EnableMaxCheck(Action callback) {
            if (ActualCount < MaxCount) {
                callback?.Invoke();
                return;
            }

            Logger.Instance.LogMessage($"Cannot create {Tag} - maximum is {MaxCount}");
        }

        private void OnDestroy() {
            ObjectControl.OnObjectRemove -= RemoveObject;
        }

    }
} //END