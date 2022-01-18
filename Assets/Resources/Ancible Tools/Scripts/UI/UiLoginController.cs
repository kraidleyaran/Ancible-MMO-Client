using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI
{
    public class UiLoginController : MonoBehaviour
    {
        private static UiLoginController _instance = null;

        [SerializeField] private InputField _usernameInput;
        [SerializeField] private InputField _passwordInput;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            SubscribeToMessages();
            gameObject.SetActive(false);
        }

        public void Login()
        {
            gameObject.SendMessage(new LoginMessage{Username = _usernameInput.text, Password = _passwordInput.text});
            _passwordInput.text = string.Empty;
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ShowLoginMessage>(ShowLogin);
            gameObject.Subscribe<HideLoginMessage>(HideLogin);
        }

        private void ShowLogin(ShowLoginMessage msg)
        {
            gameObject.SetActive(true);
        }

        private void HideLogin(HideLoginMessage msg)
        {
            _passwordInput.text = string.Empty;
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }

        
    }
}