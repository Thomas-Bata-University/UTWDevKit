using System;
using Script.Enum;
using TMPro;
using UnityEngine;

namespace Script.SceneLoad {
    public class LoadingScreen : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public static LoadingScreen Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI hullText, turretText, suspensionText, weaponryText;
        [SerializeField] private GameObject loadingScreen;

        private Coroutine _activeMessageCoroutine;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void Show() {
            hullText.text = null;
            turretText.text = null;
            suspensionText.text = null;
            weaponryText.text = null;

            loadingScreen.SetActive(true);
        }

        public void Hide() {
            loadingScreen.SetActive(false);
        }

        public void SetText(string text, TankPartType partType) {
            GetText(partType).text = text;
        }

        private TextMeshProUGUI GetText(TankPartType partType) {
            switch (partType) {
                case TankPartType.HULL:
                    return hullText;
                case TankPartType.TURRET:
                    return turretText;
                case TankPartType.SUSPENSION:
                    return suspensionText;
                case TankPartType.WEAPONRY:
                    return weaponryText;
                default:
                    throw new ArgumentOutOfRangeException(nameof(partType), partType, null);
            }
        }

    }
} //END