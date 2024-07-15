using Script.Enum;
using Script.Manager;
using Script.Static;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script.Controller {
    public class TabControl : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public GameObject[] tabs;
        public Image[] buttons;

        public Color active, inactive;

        private int _activeTab;

        private void Start() {
            SwitchTab((int)TankPartType.Hull);
        }

        public void SwitchTab(int index) {
            foreach (var tab in tabs) {
                tab.SetActive(false);
            }

            tabs[index].SetActive(true);
            _activeTab = index;

            foreach (var button in buttons) {
                button.color = inactive;
            }

            buttons[index].color = active;
        }

        public void CreateNewPart() {
            ProjectManager.Instance.partType = (TankPartType)_activeTab;
            SceneManager.LoadScene(SceneNames.Editor);
        }

    }
} //END