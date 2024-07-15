using System;
using Script.Enum;
using TMPro;
using UnityEngine;

namespace Script.Component.Parts {
    public class TransformComponent : AComponent {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        private const string FORMAT = "0.##";

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
            SetPosition(objectInstance.position);
            SetRotation(objectInstance);

            RegisterListeners(positionX, Axis.X, OnPositionChanged);
            RegisterListeners(positionY, Axis.Y, OnPositionChanged);
            RegisterListeners(positionZ, Axis.Z, OnPositionChanged);

            RegisterListeners(rotationX, Axis.X, OnRotationChanged);
            RegisterListeners(rotationY, Axis.Y, OnRotationChanged);
            RegisterListeners(rotationZ, Axis.Z, OnRotationChanged);
        }

        void RegisterListeners(TMP_InputField inputField, Axis axis, Action<string, Axis> callback) {
            inputField.onSubmit.AddListener(value => callback(value, axis));
            inputField.onDeselect.AddListener(value => callback(value, axis));
        }

        protected override void UpdateImpl() {
            if (IsObjectMoving) {
                SetPosition(objectInstance.position);
                SetRotation(objectInstance);
            }
        }

        public void ResetPosition() {
            objectInstance.position = Vector3.zero;
            SetPosition(objectInstance.position);
        }

        public void ResetRotation() {
            objectInstance.rotation = Quaternion.Euler(Vector3.zero);
            SetRotation(objectInstance);
        }

        private void OnPositionChanged(string value, Axis axis) {
            var position = CalculateTransform(value, axis, objectInstance.position);

            objectInstance.position = position;
            SetPosition(position);
        }

        private void OnRotationChanged(string value, Axis axis) {
            var rotation = Quaternion.Euler(CalculateTransform(value, axis, objectInstance.rotation.eulerAngles));

            objectInstance.rotation = rotation;
            SetRotation(objectInstance);
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

        private void SetRotation(Transform rotation) {
            var angle = rotation.eulerAngles;

            rotationX.text = InspectorRotation(angle.x).ToString(FORMAT);
            rotationY.text = InspectorRotation(angle.y).ToString(FORMAT);
            rotationZ.text = InspectorRotation(angle.z).ToString(FORMAT);
        }

        private float InspectorRotation(float angle) {
            if (angle > 180) {
                angle -= 360;
            }

            return angle;
        }

    }
} //END