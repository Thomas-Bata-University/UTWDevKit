using Script.Enum;
using TMPro;
using UnityEngine;

namespace Script.Component.Parts {
    public class TransformComponent : AComponent {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        private const string FORMAT = "0.#####";

        [Header("Position input field")]
        public TMP_InputField positionX;
        public TMP_InputField positionY;
        public TMP_InputField positionZ;

        [Header("Rotation input field")]
        public TMP_InputField rotationX;
        public TMP_InputField rotationY;
        public TMP_InputField rotationZ;

        protected override void AwakeImpl() {
        }

        protected override void StartImpl() {
            SetPosition(objectTransform.position);
            SetRotation(objectTransform.rotation);

            positionX.onSubmit.AddListener(value => OnPositionChanged(value, Axis.X));
            positionY.onSubmit.AddListener(value => OnPositionChanged(value, Axis.Y));
            positionZ.onSubmit.AddListener(value => OnPositionChanged(value, Axis.Z));

            rotationX.onSubmit.AddListener(value => OnRotationChanged(value, Axis.X));
            rotationY.onSubmit.AddListener(value => OnRotationChanged(value, Axis.Y));
            rotationZ.onSubmit.AddListener(value => OnRotationChanged(value, Axis.Z));
        }

        protected override void UpdateImpl() {
            if (IsObjectMoving) {
                SetPosition(objectTransform.position);
                SetRotation(objectTransform.rotation);
            }
        }

        private void OnPositionChanged(string value, Axis axis) {
            var position = CalculateTransform(value, axis, objectTransform.position);

            objectTransform.position = position;
            SetPosition(position);
        }

        private void OnRotationChanged(string value, Axis axis) {
            var rotation = Quaternion.Euler(CalculateTransform(value, axis, objectTransform.rotation.eulerAngles));

            objectTransform.rotation = rotation;
            SetRotation(rotation);
        }

        private Vector3 CalculateTransform(string value, Axis axis, Vector3 transform) {
            float result = float.TryParse(value, out float parsedValue) ? parsedValue : 0;

            switch (axis) {
                case Axis.X:
                    transform.x = result;
                    break;
                case Axis.Y:
                    transform.y = result;
                    break;
                case Axis.Z:
                    transform.z = result;
                    break;
            }

            return transform;
        }

        private void SetPosition(Vector3 position) {
            positionX.text = position.x.ToString(FORMAT);
            positionY.text = position.y.ToString(FORMAT);
            positionZ.text = position.z.ToString(FORMAT);
        }

        private void SetRotation(Quaternion rotation) {
            var euler = rotation.eulerAngles;
            rotationX.text = euler.x.ToString(FORMAT);
            rotationY.text = euler.y.ToString(FORMAT);
            rotationZ.text = euler.z.ToString(FORMAT);
        }

    }
} //END