using System;
using System.Collections.Generic;
using Script.Enum;
using UnityEngine;

namespace Script.TankPart {
    public abstract class ATankPart : ScriptableObject {

        /// <summary>
        /// KEY - Type of the component | VALUE - Component prefab
        /// </summary>
        private readonly Dictionary<ComponentType, GameObject> _prefabDictionary = new();

        [Serializable]
        public struct PrefabEntry {

            public ComponentType componentType;
            public GameObject prefab;

        }

        public PrefabEntry[] prefabEntry;

        public Dictionary<ComponentType, GameObject> Initialize() {
            foreach (var entry in prefabEntry) {
                if (!_prefabDictionary.ContainsKey(entry.componentType))
                    _prefabDictionary.Add(entry.componentType, entry.prefab);
            }

            return _prefabDictionary;
        }

        public GameObject GetPrefab(ComponentType type) {
            if (_prefabDictionary.TryGetValue(type, out GameObject prefab)) {
                return prefab;
            }

            return null;
        }

        public Dictionary<ComponentType, GameObject> GetData() {
            return _prefabDictionary;
        }

    }
} //END