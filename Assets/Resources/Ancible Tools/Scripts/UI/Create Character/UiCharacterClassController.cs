using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.System.CharacterClasses;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Create_Character
{
    public class UiCharacterClassController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform RectTransform;
        [SerializeField] private Text _nameText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _frameImage;

        public CharacterClass CharacterClass { get; private set; }

        private bool _hovered = false;
        private bool _selected = false;

        public void Setup(CharacterClass characterClass)
        {
            CharacterClass = characterClass;
            _nameText.text = CharacterClass.DisplayName;
            _iconImage.sprite = characterClass.Icon;
            
        }

        public void Select()
        {
            _selected = true;
            _frameImage.color = UiController.SelectColor;
            var setSelectedCharacterClassMsg = MessageFactory.GeneratedSetSelectedCharacterClassMsg();
            setSelectedCharacterClassMsg.Controller = this;
            gameObject.SendMessage(setSelectedCharacterClassMsg);
            MessageFactory.CacheMessage(setSelectedCharacterClassMsg);
        }

        public void Unselect()
        {
            _selected = false;
            _frameImage.color = _hovered ? UiController.HoverColor : Color.white;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
            if (!_selected)
            {
                _frameImage.color = UiController.HoverColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
            if (!_selected)
            {
                _frameImage.color = Color.white;
            }
        }
    }
}