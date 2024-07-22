using Script.Buttons;
using Script.Static;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Action = Script.Enum.Action;

namespace Script.Controller {
    public class ControlPanel : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public UnityAction<Action> OnActionChange;

        public Button[] actionButtons;

        public Button activeActionButton;

        private void Start() {
            OnButtonClickAction(activeActionButton);

            foreach (Button button in actionButtons) {
                button.onClick.AddListener(() => OnButtonClickAction(button));
            }
        }

        public Action GetAction() {
            return activeActionButton.gameObject.GetComponent<ButtonAction>().action;
        }

        private void OnButtonClickAction(Button clickedButton) {
            if (activeActionButton != null) {
                activeActionButton.interactable = true;
            }

            activeActionButton = clickedButton;
            activeActionButton.interactable = false;

            OnActionChange?.Invoke(GetAction());
        }

        public void Back() {
            SceneManager.LoadScene(SceneNames.Preview);
        }

    }
} //END