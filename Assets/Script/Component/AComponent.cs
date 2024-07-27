using UnityEngine;

namespace Script.Component {
    /// <summary>
    /// DO NOT USE UNITY LIFECYCLE METHODS, instead use Impl methods.
    /// </summary>
    public abstract class AComponent : MonoBehaviour {

        protected Transform ObjectInstance; //Selected object of the component
        protected GameObject ComponentGrid;

        protected bool IsObjectMoving;

        private Vector3 _lastPosition;
        private Quaternion _lastRotation;

        private void Awake() {
            AwakeImpl();
        }

        private void Start() {
            _lastPosition = ObjectInstance.position;
            _lastRotation = ObjectInstance.rotation;

            StartImpl();
        }

        private void Update() {
            if (ObjectInstance.position != _lastPosition) {
                _lastPosition = ObjectInstance.position;
                IsObjectMoving = true;
            }
            else if (ObjectInstance.rotation != _lastRotation) {
                _lastRotation = ObjectInstance.rotation;
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

        public void Initialize(Transform objectInstance, GameObject componentGrid) {
            this.ObjectInstance = objectInstance;
            this.ComponentGrid = componentGrid;
        }

    }
} //END