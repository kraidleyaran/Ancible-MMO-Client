using System;
using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.System.CharacterClasses;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Create_Character
{
    public class UiCreateCharacterController : MonoBehaviour
    {
        [SerializeField] private InputField _nameInputField;
        [SerializeField] private RectTransform _content;
        [SerializeField] private UiCharacterClassController _characterClassTemplate;
        [SerializeField] private Image _characterSpriteIcon;
        [SerializeField] private Button _createCharacterButton;

        public UiCharacterClassController Selected { get; private set; }

        private List<UiCharacterClassController> _controllers = new List<UiCharacterClassController>();

        void Awake()
        {
            var classes = CharacterClassFactoryController.StartingClasses.ToArray();
            for (var i = 0; i < classes.Length; i++)
            {
                var controller = Instantiate(_characterClassTemplate, _content);
                controller.Setup(classes[i]);
                _controllers.Add(controller);
            }
            SubscribeToMessages();
            _createCharacterButton.interactable = false;
        }

        void Start()
        {
            if (_controllers.Count > 0)
            {
                var selected = _controllers.Count > 1 ? _controllers[Random.Range(0, _controllers.Count)] : _controllers[0];
                selected.Select();
            }
        }

        public void CreateCharacter()
        {
            UiServerStatusTextController.SetText("Sending create character request to server...");
            ClientController.SendMessageToServer(new ClientCreateCharacterRequestMessage{Name = _nameInputField.text, Class = Selected.CharacterClass.name});
        }

        public void Close()
        {
            gameObject.SendMessage(CloseCharacterCreateMessage.INSTANCE);
        }

        public void NameInputUpdate()
        {
            var nameInput = _nameInputField.text;
            var cleanName = new string(nameInput.Where(char.IsLetter).ToArray());
            _nameInputField.SetTextWithoutNotify(cleanName);
            _createCharacterButton.interactable = cleanName.Length > 3;
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<SetSelectedCharacterClassMessage>(SetSelectedCharacterClass);
        }

        private void SetSelectedCharacterClass(SetSelectedCharacterClassMessage msg)
        {
            if (!Selected || Selected != msg.Controller)
            {
                if (Selected)
                {
                    Selected.Unselect();
                }
                Selected = msg.Controller;
                _characterSpriteIcon.sprite = Selected.CharacterClass.Sprites[0].Icon;
            }
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}