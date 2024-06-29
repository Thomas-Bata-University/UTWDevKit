using Script.Static;
using UnityEngine;

namespace Script.Controller {
    public class ObjectControl : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        private Camera _mainCamera;
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

        private void Start() {
            _mainCamera = GetComponent<Camera>();
        }

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
                    MoveObject();
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
            }
        }

        private void SelectObject() {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit)) {
                if (!hit.collider.CompareTag(Tags.Selectable)) return;

                if (_selectedObject == hit.transform) return;

                if (_selectedObject != hit.transform || _selectedObject is null) {
                    Outline(_selectedObject, false);
                    Outline(hit.transform, true);

                    _selectedObject = hit.transform;
                }
            }
            else {
                DeselectObject();
            }
        }

        private void DeselectObject() {
            if (_selectedObject is null) return;
            Outline(_selectedObject, false);
            _selectedObject = null;
        }

        private void MoveObject() {
            if (_isHoldingObject) {
                var screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _distance);
                var pos = _mainCamera.ScreenToWorldPoint(screenPosition);
                _selectedObject.position = pos + _offset;
            }
        }

        /// <summary>
        /// Set distance from main camera to object.
        /// Set offset from object middle to mouse position.
        /// </summary>
        private void SetDistance() {
            if (_selectedObject is null) return;
            var position = _selectedObject.position;
            var screenPoint = _mainCamera.WorldToScreenPoint(position);
            _distance = screenPoint.z;
            _offset = position -
                      _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                          _distance));
        }

        /// <summary>
        /// Checks if the selected object has been hit.
        /// </summary>
        private void SetHoldingObject() {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit)) {
                if (hit.transform == _selectedObject) {
                    _isHoldingObject = true;
                }
                else {
                    _isHoldingObject = false;
                }
            }
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

    }
} //END