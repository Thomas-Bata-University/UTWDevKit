using Script.Controller;
using Script.Enum;
using UnityEngine;

namespace Script.Component {
    /// <summary>
    /// DO NOT USE UNITY LIFECYCLE METHODS, instead use Impl methods.
    /// </summary>
    public abstract class AComponent : MonoBehaviour {

        public string componentName;
        [HideInInspector] public Transform objectInstance; //Selected object of the component
        [HideInInspector] public ComponentType type;
        [HideInInspector] public ComponentControl componentControl;

        protected bool IsObjectMoving;

        private Vector3 _lastPosition;
        private Quaternion _lastRotation;

        private void Awake() {
            AwakeImpl();
        }

        private void Start() {
            _lastPosition = objectInstance.position;
            _lastRotation = objectInstance.rotation;

            StartImpl();
        }

        private void Update() {
            if (objectInstance.position != _lastPosition) {
                _lastPosition = objectInstance.position;
                IsObjectMoving = true;
            }
            else if (objectInstance.rotation != _lastRotation) {
                _lastRotation = objectInstance.rotation;
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

        public void Initialize(Transform objectInstance, ComponentType type, ComponentControl componentControl) {
            this.objectInstance = objectInstance;
            this.type = type;
            this.componentControl = componentControl;
        }

    }
} //END