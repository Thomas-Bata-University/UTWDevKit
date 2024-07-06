using System.Collections.Generic;
using Script.Component;
using Script.Manager;
using Script.Static;
using UnityEngine;
using ComponentEnum = Script.Enum.Component;

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
        private Dictionary<GameObject, ComponentData> _componentList = new();

        public Transform componentPanel;
        public GameObject componentGridPrefab;

        public ComponentPrefabManager componentPrefabManager;

        private void Awake() {
            componentPrefabManager.Initialize();
        }

        private void Start() {
            foreach (var selectable in GameObject.FindGameObjectsWithTag(Tags.Selectable)) {
                var componentGrid = Instantiate(componentGridPrefab, componentPanel);
                componentGrid.name = selectable.name + "_componentGrid"; //TODO check if name already exists

                List<AComponent> componentList = new();
                componentList.Add(CreateComponent(componentGrid, selectable, ComponentEnum.Transform));
                _componentList.Add(selectable, new ComponentData(selectable, componentGrid, componentList));
                componentGrid.SetActive(false);
            }
        }

        public void AddComponent(GameObject selectedObject, ComponentEnum type) {
            //TODO check if component is already created
            var componentGrid = _componentList[selectedObject].ComponentGrid;
            _componentList[selectedObject].Components.Add(CreateComponent(componentGrid, selectedObject, type));
        }

        public void EnableComponents(Transform selectedObject) {
            if (selectedObject is null) return;
            _componentList[selectedObject.gameObject].ComponentGrid.SetActive(true);
        }

        public void DisableComponents(Transform selectedObject) {
            if (selectedObject is null) return;
            _componentList[selectedObject.gameObject].ComponentGrid.SetActive(false);
        }

        private AComponent CreateComponent(GameObject componentGrid, GameObject selectedObject, ComponentEnum type) {
            var prefab = componentPrefabManager.GetPrefab(type);
            var component = Instantiate(prefab, componentGrid.transform);
            var componentScript = component.GetComponent<AComponent>();
            componentScript.objectInstance = selectedObject;
            component.name = componentScript.componentName; //TODO check if name already exists
            return componentScript;
        }

    }
} //END