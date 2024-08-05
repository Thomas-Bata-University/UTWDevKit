using TMPro;
using UnityEngine;

namespace Script.Other {
    public class BoundCamera : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        private Camera _camera;
        public GameObject info;

        private void Start() {
            _camera = GetComponent<Camera>();
        }

        public Bounds GetTargetsBounds(Transform[] targets) {
            if (targets.Length == 0) {
                info.SetActive(true);
                return new Bounds();
            }

            info.SetActive(false);
            Bounds bounds = new Bounds(targets[0].transform.position, Vector3.zero);
            foreach (var target in targets) {
                bounds.Encapsulate(target.GetComponent<Renderer>().bounds);
            }

            return bounds;
        }

        public void SetCamera(Bounds bounds) {
            _camera.transform.position = bounds.center + Vector3.one * 10f;
            _camera.transform.LookAt(bounds.center);

            float size = bounds.size.magnitude;
            _camera.orthographicSize = size / 2;
        }

        public void Restart() {
            _camera.targetTexture.Release();
        }

    }
} //END