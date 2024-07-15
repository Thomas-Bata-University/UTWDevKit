using System;
using Script.Enum;
using Script.Manager;
using Script.Part;
using UnityEngine;

namespace Script.Controller {
    public class TankPartControl : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public TaskControl taskControl;

        [Serializable]
        public struct PrefabEntry {

            public TankPartType partType;
            public TankPart part;

        }

        public PrefabEntry[] prefabEntry;

        private void Start() {
            foreach (var entry in prefabEntry) {
                if (entry.partType == ProjectManager.Instance.partType) {
                    taskControl.Initialize(entry.part.tasks);
                    return;
                }
            }
        }

    }
} //END