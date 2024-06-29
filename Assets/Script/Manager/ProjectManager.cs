using Script.Static;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Manager {
    public class ProjectManager : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public void CreateNewProject() {
            Debug.Log("Creating new project.");
            SceneManager.LoadScene(SceneNames.Editor);
        }

        public void ImportExistingProject() {
        }

    }
} //END