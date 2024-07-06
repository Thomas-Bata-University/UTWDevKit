using UnityEngine;

namespace Script.Controller {
    public class CamerControl : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        private Camera _mainCamera;

        [Header("Rotate")]
        public Transform orientation;
        public float rotationSpeed = 1.5f;

        [Header("Movement")]
        public float acceleration = 0.5f;
        public float currentSpeed = 0f;
        public float maxSpeed = 5f;
        public float dragSpeed = 40f;
        public float zoomSpeed = 3f;

        private void Start() {
            _mainCamera = GetComponent<Camera>();
        }

        private void Update() {
            Zoom();

            if (Input.GetMouseButton(1)) {
                RotateAround(_mainCamera.transform);
                MoveCamera();
            }

            if (Input.GetMouseButton(2)) {
                MoveCameraWithMouse();
            }
        }

        private void MoveCamera() {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveDirection = Vector3.zero;

            if (Mathf.Abs(horizontalInput) > 0.01f || Mathf.Abs(verticalInput) > 0.01f) {
                moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
            }

            if (Input.GetKey(KeyCode.Q)) {
                moveDirection += Vector3.down;
            }
            else if (Input.GetKey(KeyCode.E)) {
                moveDirection += Vector3.up;
            }

            float multiplicator = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed * multiplicator, acceleration * Time.deltaTime);

            _mainCamera.transform.Translate(moveDirection * (currentSpeed * Time.deltaTime));
        }

        private void MoveCameraWithMouse() {
            _mainCamera.transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed,
                -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed, 0);
        }

        private void RotateAround(Transform center) {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

            Transform cameraTransform = _mainCamera.transform;
            Vector3 position = center.position;

            cameraTransform.RotateAround(position, Vector3.up, mouseX);
            cameraTransform.RotateAround(position, cameraTransform.right, -mouseY);

            orientation.rotation = Quaternion.Inverse(cameraTransform.rotation);
        }

        private void Zoom() {
            _mainCamera.transform
                .Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, Space.Self);
        }

    }
} //END