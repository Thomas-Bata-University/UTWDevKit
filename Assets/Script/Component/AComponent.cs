using UnityEngine;

namespace Script.Component {
    /// <summary>
    /// DO NOT USE UNITY LIFECYCLE METHODS, instead use Impl methods.
    /// </summary>
    public abstract class AComponent : MonoBehaviour {

        public string componentName;
        [HideInInspector]
        public GameObject objectInstance; //Selected object of the component
        [HideInInspector]
        public Transform objectTransform;

        protected bool IsObjectMoving;

        private Vector3 _lastPosition;

        private void Awake() {
            AwakeImpl();
        }

        private void Start() {
            objectTransform = objectInstance.transform;
            _lastPosition = objectTransform.position;

            StartImpl();
        }

        private void Update() {
            if (objectTransform.position != _lastPosition) {
                _lastPosition = objectTransform.position;
                IsObjectMoving = true;
            }
            else {
                IsObjectMoving = false;
            }

            UpdateImpl();
        }

        protected abstract void AwakeImpl();

        protected abstract void StartImpl();

        protected abstract void UpdateImpl();

    }
} //END