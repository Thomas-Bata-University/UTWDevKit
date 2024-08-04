using System.Collections.Generic;
using Script.Enum;
using UnityEngine;

namespace Script.Manager {
    public class UniqueIdGenerator : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public static UniqueIdGenerator Instance;

        private Dictionary<TankPartType, int> _typeCounter = new();

        private int _currentID;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }

        public int GetNextID(TankPartType type) {
            _typeCounter.TryAdd(type, 1);
            return _typeCounter[type]++;
        }

        public void SetCurrentID(SaveManager.IDs id) {
            _typeCounter[id.type] = id.count;
        }

        public int GetCurrentID(TankPartType type) {
            return _typeCounter.GetValueOrDefault(type, 0);
        }

    }
} //END