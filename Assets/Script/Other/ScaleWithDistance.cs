using Script.Static;
using UnityEngine;

public class ScaleWithDistance : MonoBehaviour {

    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    private Camera _mainCamera;
    public float scaleFactor = 1.0f;
    public float minScale = 0.1f;
    public float maxScale = 100.0f;

    private void Start() {
        _mainCamera = GameObject.FindWithTag(Tags.MainCamera).GetComponent<Camera>();
    }

    void Update() {
        float distance = Vector3.Distance(transform.position, _mainCamera.transform.position);
        float scale = Mathf.Clamp(distance * scaleFactor, minScale, maxScale);
        transform.localScale = new Vector3(scale, scale, scale);
    }

} //END