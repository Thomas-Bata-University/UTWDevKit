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

        [Header("Mouse")]
        public float holdThreshold = 0.2f; // How long must be button pressed
        public float moveThreshold = 5f; // If it is less than this, it is considered immobility
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

            if (Input.GetMouseButton(0)) {
                if (Vector3.Distance(_mouseDownPosition, Input.mousePosition) > moveThreshold) {
                    _isMovingMouse = true;
                    MoveObject();
                }
                else {
                    if (!_isHoldingMouse && Time.time - _mouseDownTime >= holdThreshold) {
                        _isHoldingMouse = true;
                        Select();
                    }
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                if (!_isMovingMouse && !_isHoldingMouse) {
                    Select();
                }

                _isHoldingMouse = false;
                _isMovingMouse = false;
            }
        }

        private void Select() {
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

        private void SetDistance() {
            if (_selectedObject is null) return;
            var position = _selectedObject.position;
            _offset = position - _mainCamera.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                    _mainCamera.WorldToScreenPoint(position).z));

            _distance = Vector3.Distance(_mainCamera.transform.position, position);
        }

        private void Outline(Transform selectedObject, bool enabled) {
            if (selectedObject is not null)
                selectedObject.GetComponent<Outline>().enabled = enabled;
        }

    }
} //END