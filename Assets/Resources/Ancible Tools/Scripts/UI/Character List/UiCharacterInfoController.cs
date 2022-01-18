using AncibleCoreCommon.CommonData.Client;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Ancible_Tools.Scripts.System.Maps;
using Assets.Resources.Ancible_Tools.Scripts.System.CharacterClasses;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Character_List
{
    public class UiCharacterInfoController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform RectTransform;
        [SerializeField] private Text _characterNameText;
        [SerializeField] private Image _classSprite;
        [SerializeField] private Text _classNameText;
        [SerializeField] private Text _levelText;
        [SerializeField] private Text _mapText;
        [SerializeField] private Image _frameImage;

        public ClientCharacterInfoData Data { get; private set; }

        private bool _hovered = false;
        private bool _selected = false;

        public void Setup(ClientCharacterInfoData info)
        {
            Data = info;
            _characterNameText.text = info.Name;
            var characterClass = CharacterClassFactoryController.GetClassByName(info.Class);
            if (characterClass)
            {
                _classSprite.sprite = characterClass.Icon;
                _classNameText.text = characterClass.DisplayName;
            }

            _levelText.text = $"Level {info.Level + 1}";
            var map = MapFactoryController.GetWorldMapByName(info.Map);
            _mapText.text = map ? map.DisplayName : info.Name;
        }

        public void Select()
        {
            _selected = true;
            _frameImage.color = UiController.SelectColor;
            var setSelectedCharacterInfoMsg = MessageFactory.GenerateSetSelectedPlayerCharacterInfoMsg();
            setSelectedCharacterInfoMsg.Controller = this;
            gameObject.SendMessage(setSelectedCharacterInfoMsg);
            MessageFactory.CacheMessage(setSelectedCharacterInfoMsg);
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