using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.Client;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Server.Items;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Loot
{
    public class UiLootItemController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private Image _itemFrame;
        [SerializeField] private Text _stackText;

        public string ItemId { get; set; }

        private Item _item = null;
        private ClientLootItemData _data = null;
        private string _objectId = string.Empty;

        private bool _hovered = false;

        public void Setup(ClientLootItemData data, string objectId)
        {
            _item = ItemFactoryController.GetItemByName(data.Item);
            _data = data;
            _objectId = objectId;
            ItemId = data.Id;
            if (_item)
            {
                _itemIcon.sprite = _item.Icon;
                _itemFrame.color = ColorFactoryController.GetColorFromItemRairty(_item.Rarity);
                _stackText.text = _data.Stack > 1 ? $"{_data.Stack}" : string.Empty;
            }

            if (_hovered)
            {
                var removeHoveredInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
                removeHoveredInfoMsg.Owner = gameObject;
                gameObject.SendMessage(removeHoveredInfoMsg);
                MessageFactory.CacheMessage(removeHoveredInfoMsg);

                var setItemHoverInfoMsg = MessageFactory.GenerateSetItemHoverInfoMsg();
                setItemHoverInfoMsg.Item = _item;
                setItemHoverInfoMsg.Stack = _data.Stack;
                setItemHoverInfoMsg.Owner = gameObject;
                MessageFactory.CacheMessage(setItemHoverInfoMsg);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;

            var setItemHoverInfoMsg = MessageFactory.GenerateSetItemHoverInfoMsg();
            setItemHoverInfoMsg.Item = _item;
            setItemHoverInfoMsg.Stack = _data.Stack;
            setItemHoverInfoMsg.Owner = gameObject;
            MessageFactory.CacheMessage(setItemHoverInfoMsg);

            var setHoveredLootItemMsg = MessageFactory.GenerateSetHoveredLootItemMsg();
            setHoveredLootItemMsg.Controller = this;
            gameObject.SendMessage(setHoveredLootItemMsg);
            MessageFactory.CacheMessage(setHoveredLootItemMsg);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;

            var removeHoveredLootItemMsg = MessageFactory.GenerateRemoveHoveredLootItemMsg();
            removeHoveredLootItemMsg.Controller = this;
            gameObject.SendMessage(removeHoveredLootItemMsg);
            MessageFactory.CacheMessage(removeHoveredLootItemMsg);

            var removeHoveredInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
            removeHoveredInfoMsg.Owner = gameObject;
            gameObject.SendMessage(removeHoveredInfoMsg);
            MessageFactory.CacheMessage(removeHoveredInfoMsg);
        }

        public void OnClick()
        {
            ClientController.SendMessageToServer(new ClientLootItemRequestMessage{ObjectId = _objectId, ItemId = ItemId});
        }

        void OnDestroy()
        {
            if (_hovered)
            {
                var removeHoveredInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
                removeHoveredInfoMsg.Owner = gameObject;
                gameObject.SendMessage(removeHoveredInfoMsg);
                MessageFactory.CacheMessage(removeHoveredInfoMsg);
            }
        }
    }
}