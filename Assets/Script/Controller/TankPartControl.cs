using System;
using Script.Enum;
using Script.Manager;
using UnityEngine;

namespace Script.Controller {
    public class TankPartControl : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public TankPartType partType = ProjectManager.Instance.partType;

        public ComponentControl componentControl;

        [Serializable]
        public struct PrefabEntry {

            public TankPartType partType;
            public GameObject prefab;

        }

        public PrefabEntry[] prefabEntry;

        private void Start() {
            foreach (var entry in prefabEntry) {
                if (entry.partType == partType) {
                    var selectableObject = Instantiate(entry.prefab);
                    componentControl.CreateGridForObject(selectableObject.transform);
                    return;
                }
            }
        }

    }
} //END