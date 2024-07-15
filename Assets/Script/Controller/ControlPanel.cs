using Script.Buttons;
using Script.Static;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Action = Script.Enum.Action;

namespace Script.Controller {
    public class ControlPanel : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public Button[] actionButtons;

        public Button activeActionButton;

        public struct ActionControl {

            public ActionControl(Action action) {
                Action = action;
            }

            public Action Action;

        }

        private void Start() {
            OnButtonClickAction(activeActionButton);

            foreach (Button button in actionButtons) {
                button.onClick.AddListener(() => OnButtonClickAction(button));
            }
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                OnButtonClickAction(actionButtons[0]);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                OnButtonClickAction(actionButtons[1]);
            }
        }

        public ActionControl GetControl() {
            var action = activeActionButton.gameObject.GetComponent<ButtonAction>().action;
            return new ActionControl(action);
        }

        private void OnButtonClickAction(Button clickedButton) {
            if (activeActionButton != null) {
                activeActionButton.interactable = true;
            }

            activeActionButton = clickedButton;
            activeActionButton.interactable = false;
        }

        public void Back() {
            SceneManager.LoadScene(SceneNames.Preview);
        }

    }
} //END