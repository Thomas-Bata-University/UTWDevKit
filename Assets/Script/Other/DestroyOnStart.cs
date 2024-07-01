using UnityEngine;

public class DestroyOnStart : MonoBehaviour {

    //Add comment to a script
    [TextArea(1, 5)]
    public string Notes = "Comment";

    //--------------------------------------------------------------------------------------------------------------------------

    private void Start() {
        Destroy(gameObject);
    }

} //END