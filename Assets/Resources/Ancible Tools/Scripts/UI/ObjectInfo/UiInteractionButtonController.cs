using AncibleCoreCommon;
using AncibleCoreCommon.CommonData;
using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.ObjectInfo
{
    public class UiInteractionButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public InteractionType Interaction { get; private set; }

        [SerializeField] private Image _iconImage;

        private string _objectId = string.Empty;
        private bool _hovered = false;

        public void Setup(InteractionType interaction, string objectId)
        {
            Interaction = interaction;
            _objectId = objectId;
        }

        public void SetIcon(Sprite icon)
        {
            _iconImage.sprite = icon;
        }

        public void OnClick()
        {
            ClientController.SendMessageToServer(new ClientInteractWithObjectRequestMessage { ObjectId = _objectId, Interaction = Interaction });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;

            var setGeneralHoverInfoMsg = MessageFactory.GenerateSetGeneralHoverInfoMsg();
            setGeneralHoverInfoMsg.Icon = _iconImage.sprite;
            setGeneralHoverInfoMsg.Title = Interaction.GetInteractionText();
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