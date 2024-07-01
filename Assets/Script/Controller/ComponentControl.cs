using System.Collections.Generic;
using Script.Component;
using Script.Static;
using UnityEngine;

namespace Script.Controller {
    public class ComponentControl : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        private Dictionary<GameObject, ComponentData> _componentList = new();

        public Transform componentPanel;
        public GameObject componentGridPrefab;

        public GameObject transformComponentPrefab; //TODO change logic

        private void Start() {
            foreach (var selectable in GameObject.FindGameObjectsWithTag(Tags.Selectable)) {
                var componentGrid = Instantiate(componentGridPrefab, componentPanel);
                componentGrid.name = selectable.name + "_componentGrid"; //TODO check if name already exists

                List<AComponent> componentList = new();
                componentList.Add(CreateComponent(componentGrid, selectable));
                _componentList.Add(selectable, new ComponentData(selectable, componentGrid, componentList));
                componentGrid.SetActive(false);
            }
        }

        public void AddComponent(GameObject selectedObject) {
            var componentGrid = _componentList[selectedObject].ComponentGrid;
            _componentList[selectedObject].Components.Add(CreateComponent(componentGrid, selectedObject));
        }

        public void EnableComponents(Transform selectedObject) {
            if (selectedObject is null) return;
            _componentList[selectedObject.gameObject].ComponentGrid.SetActive(true);
        }

        public void DisableComponents(Transform selectedObject) {
            if (selectedObject is null) return;
            _componentList[selectedObject.gameObject].ComponentGrid.SetActive(false);
        }

        private AComponent CreateComponent(GameObject componentGrid, GameObject selectedObject) {
            var component = Instantiate(transformComponentPrefab, componentGrid.transform); //TODO change
            var componentScript = component.GetComponent<AComponent>();
            componentScript.objectInstance = selectedObject;
            component.name = componentScript.componentName; //TODO check if name already exists
            return componentScript;
        }

    }
} //END