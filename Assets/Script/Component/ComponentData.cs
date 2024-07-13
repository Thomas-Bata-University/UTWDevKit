using System.Collections.Generic;
using UnityEngine;

namespace Script.Component {
    public class ComponentData {

        public GameObject SelectedObject { get; set; }
        public GameObject ComponentGrid { get; set; }
        public List<AComponent> Components { get; set; }

        public ComponentData(GameObject selectedObject, GameObject componentGrid, List<AComponent> components) {
            SelectedObject = selectedObject;
            ComponentGrid = componentGrid;
            Components = components;
        }

    }
} //END