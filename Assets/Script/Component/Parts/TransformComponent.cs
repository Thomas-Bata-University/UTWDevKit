using Script.Enum;
using TMPro;
using UnityEngine;

namespace Script.Component.Parts {
    public class TransformComponent : AComponent {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        private const string FORMAT = "F5";

        private Transform _transform;

        [Header("Position input field")]
        public TMP_InputField positionX;
        public TMP_InputField positionY;
        public TMP_InputField positionZ;

        [Header("Rotation input field")]
        public TMP_InputField rotationX;
        public TMP_InputField rotationY;
        public TMP_InputField rotationZ;

        private void Awake() {
            componentName = "transform_component";
        }

        private void Start() {
            _transform = objectInstance.transform;

            positionX.onValueChanged.AddListener(value => OnPositionChanged(value, Axis.X));
            positionY.onValueChanged.AddListener(value => OnPositionChanged(value, Axis.Y));
            positionZ.onValueChanged.AddListener(value => OnPositionChanged(value, Axis.Z));

            rotationX.onValueChanged.AddListener(value => OnRotationChanged(value, Axis.X));
            rotationY.onValueChanged.AddListener(value => OnRotationChanged(value, Axis.Y));
            rotationZ.onValueChanged.AddListener(value => OnRotationChanged(value, Axis.Z));
        }

        private void Update() {
            var position = _transform.position;
            positionX.text = position.x.ToString(FORMAT);
            positionY.text = position.y.ToString(FORMAT);
            positionZ.text = position.z.ToString(FORMAT);

            var rotation = _transform.rotation.eulerAngles;
            rotationX.text = rotation.x.ToString(FORMAT);
            rotationY.text = rotation.y.ToString(FORMAT);
            rotationZ.text = rotation.z.ToString(FORMAT);
        }

        private void OnPositionChanged(string value, Axis axis) {
            if (float.TryParse(value, out float result)) {
                var position = _transform.position;
                switch (axis) {
                    case Axis.X:
                        position.x = result;
                        break;
                    case Axis.Y:
                        position.y = result;
                        break;
                    case Axis.Z:
                        position.z = result;
                        break;
                }

                _transform.position = position;
            }
        }

        private void OnRotationChanged(string value, Axis axis) {
            if (float.TryParse(value, out float result)) {
                var rotation = _transform.rotation.eulerAngles;
                switch (axis) {
                    case Axis.X:
                        rotation.x = result;
                        break;
                    case Axis.Y:
                        rotation.y = result;
                        break;
                    case Axis.Z:
                        rotation.z = result;
                        break;
                }

                _transform.rotation = Quaternion.Euler(rotation);
            }
        }

    }
} //END