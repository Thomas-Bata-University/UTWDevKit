using System.Collections.Generic;
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

        public void Initialize(List<GameObject> tasks) {
            foreach (var task in tasks) {
                var taskObject = Instantiate(task, parent);
                this.tasks.Add(taskObject.GetComponent<ATask>());
            }
        }

    }
} //END