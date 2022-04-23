using AncibleCoreCommon;
using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.AccountRegistration
{
    public class UiAccountRegistrationController : MonoBehaviour
    {
        [SerializeField] private InputField _usernameText;
        [SerializeField] private InputField _passwordText;
        [SerializeField] private InputField _verifyPasswordText;
        [SerializeField] private InputField _gameKeyText;
        [SerializeField] private Button _registerButton = null;

        [SerializeField] private int _requiredPasswordLength = 5;
        [SerializeField] private int _requiredUsernameLength = 5;


        void Awake()
        {
            SubscribeToMessages();
            gameObject.SetActive(false);
        }

        public void Register()
        {
            if (string.IsNullOrWhiteSpace(_usernameText.text))
            {
                UiServerStatusTextController.SetText($"Username must be at least {_requiredUsernameLength} characters long", true);
            }
            else if (_usernameText.text.Length < 5)
            {
                UiServerStatusTextController.SetText($"Username must be at least {_requiredUsernameLength} characters long", true);
            }
            else if (string.IsNullOrWhiteSpace(_passwordText.text) || string.IsNullOrWhiteSpace(_verifyPasswordText.text))
            {
                UiServerStatusTextController.SetText($"Password must be at least {_requiredPasswordLength} characters long");
            }
            else if (_passwordText.text.Length < _requiredPasswordLength)
            {
                UiServerStatusTextController.SetText($"Password must be at least {_requiredPasswordLength} characters long");
            }
            else if (_passwordText.text != _verifyPasswordText.text)
            {
                UiServerStatusTextController.SetText("Passwords do not match", true);
            }
            else if (string.IsNullOrEmpty(_gameKeyText.text))
            {
                UiServerStatusTextController.SetText("Invalid Game Key");
            }
            else
            {
                _registerButton.interactable = false;
                gameObject.SendMessage(new RegisterKeyMessage{Username = _usernameText.text, Password = _passwordText.text, GameKey = _gameKeyText.text});
            }
        }

        public void CheckFields()
        {
            _registerButton.interactable = CheckUsername() && CheckPasswords() && CheckGameKey();
        }

        private bool CheckUsername()
        {
            return _usernameText.text.Length > _requiredUsernameLength && !string.IsNullOrWhiteSpace(_usernameText.text);
        }

        private bool CheckPasswords()
        {
            return _passwordText.text.Length > _requiredPasswordLength && _verifyPasswordText.text.Length > _requiredPasswordLength && !string.IsNullOrWhiteSpace(_passwordText.text) && _passwordText.text == _verifyPasswordText.text;
        }

        private bool CheckGameKey()
        {
            return !string.IsNullOrWhiteSpace(_gameKeyText.text);
        }

        public void Close()
        {
            ClearFields();
            gameObject.SetActive(false);
            gameObject.SendMessage(ShowLoginMessage.INSTANCE);
        }

        private void ClearFields()
        {
            _usernameText.text = string.Empty;
            _passwordText.text = string.Empty;
            _verifyPasswordText.text = string.Empty;
            _gameKeyText.text = string.Empty;
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientClaimKeyResultMessage>(ClientClaimKeyResult);
            gameObject.Subscribe<ShowAccountRegistrationMessage>(ShowAccountRegistration);
        }

        private void ClientClaimKeyResult(ClientClaimKeyResultMessage msg)
        {
            if (msg.Success)
            {
                ClearFields();   
                UiServerStatusTextController.SetText("Account registered succesfully! Login to start playing!", true);
                gameObject.SetActive(false);
                gameObject.SendMessage(ShowLoginMessage.INSTANCE);
            }
            else
            {
                UiServerStatusTextController.SetText($"Account registration failed -{msg.Message}", true);
            }
            _registerButton.interactable = true;
        }

        private void ShowAccountRegistration(ShowAccountRegistrationMessage msg)
        {
            ClearFields();
            _registerButton.interactable = false;
            gameObject.SetActive(true);
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}