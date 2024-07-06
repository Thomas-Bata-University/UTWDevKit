using System;
using System.Collections.Generic;
using UnityEngine;
using ComponentEnum = Script.Enum.Component;

namespace Script.Manager {
    [CreateAssetMenu(fileName = "PrefabManager", menuName = "Managers/PrefabManager")]
    public class ComponentPrefabManager : ScriptableObject {

        /// <summary>
        /// KEY - Type of the component | VALUE - Component prefab
        /// </summary>
        private Dictionary<ComponentEnum, GameObject> _prefabDictionary = new();

        [Serializable]
        public struct PrefabEntry {

            public ComponentEnum componentType;
            public GameObject prefab;

        }

        public PrefabEntry[] prefabEntry;

        public void Initialize() {
            foreach (var entry in prefabEntry) {
                if (!_prefabDictionary.ContainsKey(entry.componentType))
                    _prefabDictionary.Add(entry.componentType, entry.prefab);
            }
        }

        public GameObject GetPrefab(ComponentEnum type) {
            if (_prefabDictionary.TryGetValue(type, out GameObject prefab)) {
                return prefab;
            }

            return null;
        }

        public IEnumerable<GameObject> GetAllPrefabs() {
            return _prefabDictionary.Values;
        }

    }
} //END