using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;
using MessageFactory = Assets.Ancible_Tools.Scripts.System.MessageFactory;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.HoverInfo
{
    public class UiActiveGeneralInfoController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private string _title;
        [SerializeField] private Sprite _icon;
        [SerializeField] [TextArea(3, 10)] private string _description;

        private bool _hovered = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
            var setGeneralHoverInfoMsg = MessageFactory.GenerateSetGeneralHoverInfoMsg();
            setGeneralHoverInfoMsg.Title = _title;
            setGeneralHoverInfoMsg.Icon = _icon;
            setGeneralHoverInfoMsg.Description = _description;
            setGeneralHoverInfoMsg.Owner = gameObject;
            gameObject.SendMessage(setGeneralHoverInfoMsg);
            MessageFactory.CacheMessage(setGeneralHoverInfoMsg);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
            var removeHoverInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
            removeHoverInfoMsg.Owner = gameObject;
            gameObject.SendMessage(removeHoverInfoMsg);
            MessageFactory.CacheMessage(removeHoverInfoMsg);
        }

        void OnDestroy()
        {
            if (_hovered)
            {
                var removeHoverInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
                removeHoverInfoMsg.Owner = gameObject;
                gameObject.SendMessage(removeHoverInfoMsg);
                MessageFactory.CacheMessage(removeHoverInfoMsg);
            }
        }
    }
}