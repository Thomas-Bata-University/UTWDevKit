using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Log {
    public class Logger : MonoBehaviour {

        //Add comment to a script
        [TextArea(1, 5)]
        public string Notes = "Comment";

        //--------------------------------------------------------------------------------------------------------------------------

        public const string FORMAT = "HH:mm:ss";

        public static Logger Instance { get; private set; }

        private List<GameObject> _logEntries = new();

        public GameObject logPanel, logEntryPrefab;
        public Transform content;
        public ScrollRect rect;

        [Header("Colors")]
        public Color[] background;
        private int _colorIndex;

        [Header("Unread message")]
        [SerializeField] private GameObject unreadImage;
        private bool _unread;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start() {
            logPanel.SetActive(false);
        }

        public void ShowPanel() {
            logPanel.SetActive(!logPanel.activeSelf);

            ActiveUnread();
        }

        public void Clear() {
            foreach (var go in _logEntries) {
                Destroy(go);
            }

            _logEntries.Clear();

            ActiveUnread();
        }

        public void LogMessage(string message) {
            Debug.Log(message);

            CreateMessage(message, Color.white);
        }

        public void LogSuccessfulMessage(string message) {
            Debug.Log(message);

            CreateMessage(message, Color.green);
        }

        public void LogErrorMessage(string message) {
            Debug.LogError(message);

            CreateMessage(message, Color.red);

            if (!logPanel.activeInHierarchy) {
                unreadImage.SetActive(true);
                _unread = true;
            }
        }

        private void CreateMessage(string message, Color color) {
            var logEntry = Instantiate(logEntryPrefab, content);
            logEntry.GetComponent<Image>().color = background[_colorIndex];
            logEntry.GetComponentInChildren<TextMeshProUGUI>().text = FormatMessage(message, color);

            _colorIndex = (_colorIndex + 1) % background.Length;

            _logEntries.Add(logEntry);

            if (logPanel.activeInHierarchy) {
                ScrollToBottom();
            }
        }

        private string FormatMessage(string message, Color color) {
            var formatted = $"[{DateTime.Now.ToString(FORMAT)}] {message}";
            return ChangeColor(formatted, color);
        }

        private void ScrollToBottom() {
            Canvas.ForceUpdateCanvases();
            rect.verticalNormalizedPosition = 0f;
        }

        private void ActiveUnread() {
            if (_unread) {
                unreadImage.SetActive(false);
            }
        }

        private string ChangeColor(string message, Color color) {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{message}</color>";
        }

    }
} //END