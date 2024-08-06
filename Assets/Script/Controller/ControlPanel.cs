using Script.Buttons;
using Script.Manager;
using Script.Static;
using TMPro;
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

        public TextMeshProUGUI projectName;

        private void Start() {
            OnButtonClickAction(activeActionButton);

            foreach (Button button in actionButtons) {
                button.onClick.AddListener(() => OnButtonClickAction(button));
            }

            projectName.text = SaveManager.Instance.GetCoreData().projectName;
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
            AsyncLoad.Instance.Load(SceneNames.Preview); //TODO do not load again when going back
        }

        public void Save() {
            SaveManager.Instance.Save();
        }

    }
} //END