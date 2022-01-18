using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.UI.Create_Character;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Character_List
{
    public class UiCharacterListController : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private VerticalLayoutGroup _grid;
        [SerializeField] private UiCharacterInfoController _characterInfoTemplate;
        [SerializeField] private Button _enterWorldButton;

        [SerializeField] private UiCreateCharacterController _createControllerTemplate;

        private Dictionary<string, UiCharacterInfoController> _controllers = new Dictionary<string, UiCharacterInfoController>();

        private UiCharacterInfoController _selectedController = null;

        private UiCreateCharacterController _createController;

        void Awake()
        {
            SubscribeToMessages();
            gameObject.SetActive(false);
        }

        public void SelectCharacter()
        {
            UiServerStatusTextController.SetText("Sending enter world request to server...");
            ClientController.SendMessageToServer(new ClientEnterWorldWithCharacterRequestMessage{Name = _selectedController.Data.Name});
            gameObject.SendMessage(HidePlayerCharacterListMessage.INSTANCE);
        }

        public void CreateCharacter()
        {
            gameObject.SendMessage(ShowCharacterCreateMessage.INSTANCE);
        }

        private void Refresh()
        {
            var characters = DataController.PlayerCharacters.ToArray();
            for (var i = 0; i < characters.Length; i++)
            {
                if (!_controllers.TryGetValue(characters[i].Name, out var controller))
                {
                    controller = Instantiate(_characterInfoTemplate, _grid.transform);
                    _controllers.Add(characters[i].Name, controller);
                }
                controller.Setup(characters[i]);
            }

            var removed = _controllers.Keys.Where(k => characters.FirstOrDefault(c => c.Name == k) == null).ToArray();
            for (var i = 0; i < removed.Length; i++)
            {
                var controller = _controllers[removed[i]];
                _controllers.Remove(removed[i]);
                Destroy(controller.gameObject);
            }

            var height = _controllers.Count * (_grid.spacing + _characterInfoTemplate.RectTransform.rect.height) + _grid.padding.top;
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            _enterWorldButton.interactable = _selectedController;
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<RefreshPlayerCharacterListMessage>(RefreshCharacterList);
            gameObject.Subscribe<ShowPlayerCharacterListMessage>(ShowCharacterList);
            gameObject.Subscribe<HidePlayerCharacterListMessage>(HideCharacterList);
            gameObject.Subscribe<SetSelectedPlayerCharacterInfoMessage>(SetSelectPlayerCharacterInfo);
            gameObject.Subscribe<ShowCharacterCreateMessage>(ShowCharacterCreate);
            gameObject.Subscribe<CloseCharacterCreateMessage>(CloseCharacterCreate);
            gameObject.Subscribe<ClientCreateCharacterResultMessage>(ClientCreateCharacterResult);
        }

        private void RefreshCharacterList(RefreshPlayerCharacterListMessage msg)
        {
            if (gameObject.activeSelf)
            {
                Refresh();
            }
        }

        private void ShowCharacterList(ShowPlayerCharacterListMessage msg)
        {
            Refresh();
            gameObject.SetActive(true);
        }

        private void HideCharacterList(HidePlayerCharacterListMessage msg)
        {
            gameObject.SetActive(false);
        }

        private void SetSelectPlayerCharacterInfo(SetSelectedPlayerCharacterInfoMessage msg)
        {
            if (!_selectedController || _selectedController != msg.Controller)
            {
                if (_selectedController)
                {
                    _selectedController.Unselect();
                }

                _selectedController = msg.Controller;
                _enterWorldButton.interactable = _selectedController;
            }
        }

        private void ShowCharacterCreate(ShowCharacterCreateMessage msg)
        {
            if (!_createController)
            {
                _createController = Instantiate(_createControllerTemplate, transform);
            }
        }

        private void CloseCharacterCreate(CloseCharacterCreateMessage msg)
        {
            if (_createController)
            {
                Destroy(_createController.gameObject);
                _createController = null;
            }
        }

        private void ClientCreateCharacterResult(ClientCreateCharacterResultMessage msg)
        {
            if (msg.Success)
            {

                UiServerStatusTextController.SetText("Character created succesfully! Requesting character list...");
                gameObject.SendMessage(CloseCharacterCreateMessage.INSTANCE);
                ClientController.SendMessageToServer(new ClientCharacterRequestMessage());
            }
            else
            {
                UiServerStatusTextController.SetText(msg.Message, true);
            }
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}