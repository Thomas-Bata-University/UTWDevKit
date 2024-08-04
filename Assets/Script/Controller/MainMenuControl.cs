using Script.Manager;
using UnityEngine;

namespace Script.Controller {
    public class MainMenuControl : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public void CreateProject() {
            ProjectManager.Instance.CreateProject();
        }

        public void OpenProject() {
            ProjectManager.Instance.OpenProject();
        }

    }
} //END