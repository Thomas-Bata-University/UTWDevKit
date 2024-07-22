using System.Collections.Generic;
using UnityEngine;

namespace Script.SO {
    [CreateAssetMenu(fileName = "Task", menuName = "Data/Task")]
    public class TaskSO : ScriptableObject {

        #region Tasks

        [Header("Task")]
        public List<GameObject> tasks;

        #endregion

    }
} //END