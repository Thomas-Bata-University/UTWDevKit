using System.Collections.Generic;
using Script.Enum;
using Script.Static;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Action = Script.Enum.Action;

namespace Script.Controller {
    public class ObjectControl : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public static UnityAction<Transform> OnObjectSelected;

        public ComponentControl componentControl;
        public ControlPanel controlPanel;
        public Canvas canvas;
        public Camera mainCamera;
        private Transform _selectedObject;
        private float _distance;
        private Vector3 _offset;
        private bool _isHoldingObject;

        [Header("Mouse control")]
        public float holdThreshold = 0.2f; // How long the button must be pressed
        public float moveThreshold = 5f; // How much the mouse has to move
        private float _mouseDownTime;
        private Vector3 _mouseDownPosition;
        private bool _isHoldingMouse;
        private bool _isMovingMouse;

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                DeselectObject();
            }

            if (Input.GetMouseButtonDown(0)) {
                _mouseDownPosition = Input.mousePosition;
                _mouseDownTime = Time.time;
                _isHoldingMouse = false;
                _isMovingMouse = false;
                SetDistance();
                SetHoldingObject();
            }

            if (Input.GetMouseButton(0)) {
                if (IsMovingMouse()) {
                    _isMovingMouse = true;
                    switch (controlPanel.GetControl().Action) {
                        case Action.Position: {
                            MoveObject();
                        }
                            break;
                        case Action.Rotation: {
                            RotateObject();
                        }
                            break;
                    }
                }
                else {
                    if (IsHoldingMouse()) {
                        _isHoldingMouse = true;
                        SelectObject();
                    }
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                if (!_isMovingMouse && !_isHoldingMouse) {
                    SelectObject();
                }

                _isHoldingMouse = false;
                _isMovingMouse = false;
                _isHoldingObject = false;
            }
        }

        private void SelectObject() {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit)) {
                if (!hit.collider.CompareTag(Tags.Selectable)) return;

                if (_selectedObject == hit.transform) return;

                if (_selectedObject != hit.transform || _selectedObject is null) {
                    Outline(_selectedObject, false);
                    componentControl.DisableComponents(_selectedObject);

                    SetSelectedObject(hit.transform);
                    Outline(_selectedObject, true);
                    componentControl.EnableComponents(_selectedObject);
                }
            }
            else {
                if (IsPointerOverUI(Tags.ComponentPanel) || IsPointerOverUI(Tags.ControlPanel)) return;
                DeselectObject();
            }
        }

        private void DeselectObject() {
            if (_selectedObject is null) return;
            componentControl.DisableComponents(_selectedObject);
            Outline(_selectedObject, false);
            SetSelectedObject(null);
        }

        private void MoveObject() {
            if (!_isHoldingObject) return;

            var mousePosition = Input.mousePosition;
            mousePosition.z = _distance;
            var worldPos = mainCamera.ScreenToWorldPoint(mousePosition);
            var newPosition = _selectedObject.position;

            switch (controlPanel.GetControl().Axis) {
                case Axis.X: {
                    newPosition.x = worldPos.x + _offset.x;
                }
                    break;
                case Axis.Y: {
                    newPosition.y = worldPos.y + _offset.y;
                }
                    break;
                case Axis.Z: {
                    newPosition.z = worldPos.z + _offset.z;
                }
                    break;
                case Axis.Free: {
                    var screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _distance);
                    var pos = mainCamera.ScreenToWorldPoint(screenPosition);
                    _selectedObject.position = pos + _offset;
                    return;
                }
            }

            _selectedObject.position = newPosition;
        }

        private void RotateObject() {
            if (!_isHoldingObject) return;
            float mouse = 0f;
            Vector3 rotate = Vector3.zero;
            switch (controlPanel.GetControl().Axis) {
                case Axis.X: {
                    mouse = Input.GetAxis("Mouse Y");
                    rotate = Vector3.left;
                }
                    break;
                case Axis.Y: {
                    mouse = Input.GetAxis("Mouse X");
                    rotate = Vector3.up;
                }
                    break;
                case Axis.Z: {
                    mouse = Input.GetAxis("Mouse Y");
                    rotate = Vector3.forward;
                }
                    break;
                case Axis.Free: {
                    float mouseX = Input.GetAxis("Mouse X") * 2;
                    float mouseY = Input.GetAxis("Mouse Y") * 2;
                    _selectedObject.Rotate(mouseY, -mouseX, 0, Space.World);
                }
                    break;
            }

            _selectedObject.Rotate(rotate, -mouse * 2f, Space.World);
        }

        /// <summary>
        /// Set distance from main camera to object.
        /// Set offset from object middle to mouse position.
        /// </summary>
        private void SetDistance() {
            if (_selectedObject is null) return;
            var position = _selectedObject.position;
            var screenPoint = mainCamera.WorldToScreenPoint(position);
            _distance = screenPoint.z;
            _offset = position -
                      mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                          _distance));
        }

        /// <summary>
        /// Checks if the selected object has been hit.
        /// </summary>
        private void SetHoldingObject() {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit)) {
                if (hit.transform == _selectedObject) {
                    _isHoldingObject = true;
                }
                else {
                    _isHoldingObject = false;
                }
            }
        }

        private void SetSelectedObject(Transform selectedObject) {
            _selectedObject = selectedObject;
            OnObjectSelected?.Invoke(selectedObject);
        }

        /// <summary>
        /// Check that the mouse button has been pressed for longer than the threshold.
        /// </summary>
        /// <returns></returns>
        private bool IsHoldingMouse() {
            return !_isHoldingMouse && Time.time - _mouseDownTime >= holdThreshold;
        }

        /// <summary>
        /// Check if the mouse has been moved.
        /// </summary>
        /// <returns></returns>
        private bool IsMovingMouse() {
            return Vector3.Distance(_mouseDownPosition, Input.mousePosition) > moveThreshold;
        }

        private void Outline(Transform selectedObject, bool enabled) {
            if (selectedObject is not null)
                selectedObject.GetComponent<Outline>().enabled = enabled;
        }

        private bool IsPointerOverUI(string tag) {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            raycaster.Raycast(eventData, results);

            foreach (RaycastResult result in results) {
                if (result.gameObject.CompareTag(tag)) {
                    return true;
                }
            }

            return false;
        }

    }
} //END