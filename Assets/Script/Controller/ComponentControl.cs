using System.Collections.Generic;
using Script.Component;
using Script.Enum;
using Script.TankPart;
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
        private Transform _selectedObject;

        private void Start() {
            ObjectControl.OnObjectSelected += selectedObject => _selectedObject = selectedObject;
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
        /// Create grid created object.
        /// </summary>
        public void CreateGridForObject(Transform selectedObject) {
            _selectedObject = selectedObject;
            var componentGrid = Instantiate(componentGridPrefab, componentPanel);
            componentGrid.name = selectedObject.name + "_componentGrid";

            List<AComponent> componentList = new();
            var data = selectedObject.GetComponent<TankPartData>().data.Initialize();
            foreach (var pair in data) {
                componentList.Add(CreateComponent(componentGrid, selectedObject.transform, pair));
            }

            _componentList.Add(selectedObject.transform,
                new ComponentData(selectedObject.gameObject, componentGrid, componentList));
            componentGrid.SetActive(false);
        }

    }
} //END