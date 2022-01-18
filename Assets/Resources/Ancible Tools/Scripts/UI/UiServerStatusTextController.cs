using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI
{
    public class UiServerStatusTextController : MonoBehaviour
    {
        private static UiServerStatusTextController _instance = null;
        
        [SerializeField] private Text _statusText;
        [SerializeField] private Button _closeButton;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            gameObject.SetActive(false);
        }

        public void Close()
        {
            _statusText.text = string.Empty;
            _closeButton.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public static void SetText(string text, bool showClose = false)
        {
            _instance._statusText.text = $"{text}";
            _instance._closeButton.gameObject.SetActive(showClose);
            _instance.gameObject.SetActive(true);
        }

        public static void CloseText()
        {
            _instance.Close();
        }
    }
}