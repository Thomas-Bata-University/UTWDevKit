using System;
using System.Collections.Generic;
using Script.Enum;
using Script.Manager;
using Script.SO;
using Script.Task;
using UnityEngine;

namespace Script.Controller {
    public class TaskControl : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        [HideInInspector] public List<ATask> tasks = new();

        public Transform parent;

        [Serializable]
        public struct MainParts {

            public TankPartType partType;
            public TaskSO part;

        }

        public MainParts[] mainParts;

        private void Start() {
            foreach (var entry in mainParts) {
                if (entry.partType == ProjectManager.Instance.partType) {
                    Initialize(entry.part.tasks, entry.partType);
                    return;
                }
            }
        }

        private void Initialize(List<GameObject> tasks, TankPartType partType) {
            foreach (var task in tasks) {
                var taskObject = Instantiate(task, parent);
                var aTask = taskObject.GetComponent<ATask>();
                aTask.partType = partType;
                this.tasks.Add(taskObject.GetComponent<ATask>());
            }

            SaveManager.Instance.Load();
        }

    }
} //END