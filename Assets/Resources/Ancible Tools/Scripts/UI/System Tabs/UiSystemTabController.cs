using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.System.CharacterClasses;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.System_Tabs
{
    public class UiSystemTabController : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField] private Image _systemIcon;
        [SerializeField] private UiBaseWindow _window;

        public void Click()
        {
            if (_window)
            {
                UiWindowManager.ToggleWindow(_window);
            }
            else
            {
                var characterClass = CharacterClassFactoryController.GetClassByName(DataController.ActiveCharacter.PlayerClass);
                if (characterClass && characterClass.UiTalentTree)
                {
                    UiWindowManager.ToggleWindow(characterClass.UiTalentTree);
                }
            }
            
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_window)
            {
                var characterClass = CharacterClassFactoryController.GetClassByName(DataController.ActiveCharacter.PlayerClass);
                if (characterClass)
                {
                    var window = characterClass.UiTalentTree;
                    var setGeneralHoverInfoMsg = MessageFactory.GenerateSetGeneralHoverInfoMsg();
                    setGeneralHoverInfoMsg.Title = window.Title;
                    setGeneralHoverInfoMsg.Description = window.Description;
                    setGeneralHoverInfoMsg.Icon = _systemIcon.sprite;
                    setGeneralHoverInfoMsg.Owner = gameObject;
                    gameObject.SendMessage(setGeneralHoverInfoMsg);
                    MessageFactory.CacheMessage(setGeneralHoverInfoMsg);
                }
            }
            else
            {
                var setGeneralHoverInfoMsg = MessageFactory.GenerateSetGeneralHoverInfoMsg();
                setGeneralHoverInfoMsg.Title = _window.Title;
                setGeneralHoverInfoMsg.Description = _window.Description;
                setGeneralHoverInfoMsg.Icon = _systemIcon.sprite;
                setGeneralHoverInfoMsg.Owner = gameObject;
                gameObject.SendMessage(setGeneralHoverInfoMsg);
                MessageFactory.CacheMessage(setGeneralHoverInfoMsg);
            }

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var removeHoverInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
            removeHoverInfoMsg.Owner = gameObject;
            gameObject.SendMessage(removeHoverInfoMsg);
            MessageFactory.CacheMessage(removeHoverInfoMsg);
        }
    }
}