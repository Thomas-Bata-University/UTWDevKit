using System;
using Script.Static;
using UnityEngine;

namespace Script.Other {
    public class ScaleWithDistance : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        private Camera _mainCamera;
        public float scaleFactor = 1.0f;
        public float minScale = 0.1f;
        public float maxScale = 100.0f;

        private void OnEnable() {
            if (_mainCamera is null)
                _mainCamera = GameObject.FindWithTag(Tags.MainCamera).GetComponent<Camera>();
        }

        private void OnDisable() {
            _mainCamera = null;
        }

        void Update() {
            float distance = Vector3.Distance(transform.position, _mainCamera.transform.position);
            float scale = Mathf.Clamp(distance * scaleFactor, minScale, maxScale);
            transform.localScale = new Vector3(scale, scale, scale);
        }

    }
} //END