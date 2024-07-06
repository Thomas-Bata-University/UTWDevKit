using Script.Buttons;
using Script.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Controller {
    public class ControlPanel : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public Button[] actionButtons;
        public Button[] axisButtons;

        public Button activeActionButton;
        public Button activeAxisButton;

        public struct ActionControl {

            public ActionControl(Action action, Axis axis) {
                Action = action;
                Axis = axis;
            }

            public Action Action;
            public Axis Axis;

        }

        private void Start() {
            OnButtonClick(activeActionButton);
            OnButtonClicka(activeAxisButton);

            foreach (Button button in actionButtons) {
                button.onClick.AddListener(() => OnButtonClick(button));
            }

            foreach (Button button in axisButtons) {
                button.onClick.AddListener(() => OnButtonClicka(button));
            }
        }

        public ActionControl GetControl() {
            var action = activeActionButton.gameObject.GetComponent<ButtonAction>().action;
            var axis = activeAxisButton.gameObject.GetComponent<ButtonAxis>().axis;
            return new ActionControl(action, axis);
        }

        private void OnButtonClick(Button clickedButton) {
            if (activeActionButton != null) {
                activeActionButton.interactable = true;
            }

            activeActionButton = clickedButton;
            activeActionButton.interactable = false;
        }

        private void OnButtonClicka(Button clickedButton) {
            if (activeAxisButton != null) {
                activeAxisButton.interactable = true;
            }

            activeAxisButton = clickedButton;
            activeAxisButton.interactable = false;
        }

    }
} //END