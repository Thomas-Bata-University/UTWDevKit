using System.Collections.Generic;
using Script.Component;
using Script.Enum;
using Script.Task;
using Script.Utils;
using UnityEngine;

namespace Script.Controller {
    public class ComponentControl : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// KEY - Selected object | VALUE - Data about selected object like all components
        /// Component list for storing data about components for each selected object.
        /// </summary>
        private Dictionary<Transform, ComponentData> _componentList = new();

        [Header("Actual components")]
        public Transform componentPanel;
        public GameObject componentGridPrefab;

        private void Start() {
            ObjectControl.OnObjectRemove += selectedObject => {
                DisableComponents(selectedObject);
                _componentList.Remove(selectedObject);
            };
            ATask.OnPartCreation += (obj, data) => CreateGridForObject(obj, data.Initialize());
        }

        public void EnableComponents(Transform selectedObject) {
            if (selectedObject is null) return;
            _componentList[selectedObject].ComponentGrid.SetActive(true);
        }

        public void DisableComponents(Transform selectedObject) {
            if (selectedObject is null) return;
            _componentList[selectedObject].ComponentGrid.SetActive(false);
        }

        private AComponent CreateComponent(GameObject componentGrid, Transform selectedObject,
            KeyValuePair<ComponentType, GameObject> pair) {
            var component = Instantiate(pair.Value, componentGrid.transform.GetChild(0).GetChild(0));
            var componentScript = component.GetComponent<AComponent>();
            componentScript.Initialize(selectedObject, pair.Key, this);
            component.name = componentScript.componentName;
            return null;
        }

        /// <summary>
        /// Create grid for created object.
        /// </summary>
        public void CreateGridForObject(Transform parentObject, Dictionary<ComponentType, GameObject> data) {
            var componentGrid = Instantiate(componentGridPrefab, componentPanel);
            componentGrid.name = parentObject.name + "_componentGrid";

            List<AComponent> componentList = new();
            var obj = ObjectUtils.GetReference(parentObject.transform);
            foreach (var pair in data) {
                componentList.Add(CreateComponent(componentGrid, obj, pair));
            }

            _componentList.Add(obj,
                new ComponentData(parentObject.gameObject, componentGrid, componentList));
            componentGrid.SetActive(false);
        }

    }
} //END