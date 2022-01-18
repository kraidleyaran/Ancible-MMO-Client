using AncibleCoreCommon.CommonData.Client;
using AncibleCoreCommon.CommonData.Items;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Server.Items;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Character
{
    public class UiEquippedItemController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public EquippableSlot Slot;

        [SerializeField] private Image _itemIconImage;
        [SerializeField] private Image _emptySlotIconImage;
        [SerializeField] private UiCooldownController _cooldownController;

        public EquippableItem Item { get; private set; }
        public ClientEquippedItemData Data { get; private set; }

        private bool _hovered = false;

        void Awake()
        {
            _cooldownController.WakeUp();
        }

        public void Setup(ClientEquippedItemData data)
        {
            var item = ItemFactoryController.GetItemByName(data.Item);
            if (item && item is EquippableItem equippable)
            {
                _cooldownController.SetupGlobalCooldown();
                Item = equippable;
                Data = data;
                _itemIconImage.sprite = Item.Icon;
                _emptySlotIconImage.gameObject.SetActive(false);
                _itemIconImage.gameObject.SetActive(true);
                if (_hovered)
                {
                    var removeHoverInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
                    removeHoverInfoMsg.Owner = gameObject;
                    gameObject.SendMessage(removeHoverInfoMsg);
                    MessageFactory.CacheMessage(removeHoverInfoMsg);

                    var setItemHoverInfoMsg = MessageFactory.GenerateSetItemHoverInfoMsg();
                    setItemHoverInfoMsg.Item = Item;
                    setItemHoverInfoMsg.Stack = 1;
                    setItemHoverInfoMsg.Owner = gameObject;
                    gameObject.SendMessage(setItemHoverInfoMsg);
                    MessageFactory.CacheMessage(setItemHoverInfoMsg);

                    //var setGeneralHoverInfoMsg = MessageFactory.GenerateSetGeneralHoverInfoMsg();
                    //setGeneralHoverInfoMsg.Title = Item.DisplayName;
                    //setGeneralHoverInfoMsg.Description = Item.GetDescription();
                    //setGeneralHoverInfoMsg.Icon = Item.Icon;
                    //setGeneralHoverInfoMsg.Owner = gameObject;
                    //gameObject.SendMessage(setGeneralHoverInfoMsg);
                    //MessageFactory.CacheMessage(setGeneralHoverInfoMsg);
                }
            }
            else
            {
                Clear();
            }
        }

        public void Clear()
        {
            if (_hovered)
            {
                var removeHoverInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
                removeHoverInfoMsg.Owner = gameObject;
                gameObject.SendMessage(removeHoverInfoMsg);
                MessageFactory.CacheMessage(removeHoverInfoMsg);

                var setGeneralHoverInfoMsg = MessageFactory.GenerateSetGeneralHoverInfoMsg();
                setGeneralHoverInfoMsg.Title = $"{Slot}";
                setGeneralHoverInfoMsg.Description = $"Equip an item to this slot by right clicking on it in your inventory";
                setGeneralHoverInfoMsg.Icon = _emptySlotIconImage.sprite;
                setGeneralHoverInfoMsg.Owner = gameObject;
                gameObject.SendMessage(setGeneralHoverInfoMsg);
                MessageFactory.CacheMessage(setGeneralHoverInfoMsg);
            }
            _cooldownController.Clear();
            _emptySlotIconImage.gameObject.SetActive(true);
            _itemIconImage.gameObject.SetActive(false);
            Item = null;
            Data = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
            var setHoveredEquippedItemMsg = MessageFactory.GenerateSetHoveredEquippedItemMsg();
            setHoveredEquippedItemMsg.Controller = this;
            gameObject.SendMessage(setHoveredEquippedItemMsg);
            MessageFactory.CacheMessage(setHoveredEquippedItemMsg);
            if (Data != null && Item)
            {
                var setItemHoverInfoMsg = MessageFactory.GenerateSetItemHoverInfoMsg();
                setItemHoverInfoMsg.Item = Item;
                setItemHoverInfoMsg.Stack = 1;
                setItemHoverInfoMsg.Owner = gameObject;
                gameObject.SendMessage(setItemHoverInfoMsg);
                MessageFactory.CacheMessage(setItemHoverInfoMsg);
                //var setGeneralHoverInfoMsg = MessageFactory.GenerateSetGeneralHoverInfoMsg();
                //setGeneralHoverInfoMsg.Title = Item.DisplayName;
                //setGeneralHoverInfoMsg.Description = Item.GetDescription();
                //setGeneralHoverInfoMsg.Icon = Item.Icon;
                //setGeneralHoverInfoMsg.Owner = gameObject;
                //gameObject.SendMessage(setGeneralHoverInfoMsg);
                //MessageFactory.CacheMessage(setGeneralHoverInfoMsg);
            }
            else
            {
                var setGeneralHoverInfoMsg = MessageFactory.GenerateSetGeneralHoverInfoMsg();
                setGeneralHoverInfoMsg.Title = $"{Slot}";
                setGeneralHoverInfoMsg.Description = $"Equip an item to this slot by right clicking on it in your inventory";
                setGeneralHoverInfoMsg.Icon = _emptySlotIconImage.sprite;
                setGeneralHoverInfoMsg.Owner = gameObject;
                gameObject.SendMessage(setGeneralHoverInfoMsg);
                MessageFactory.CacheMessage(setGeneralHoverInfoMsg);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
            var removeHoverInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
            removeHoverInfoMsg.Owner = gameObject;
            gameObject.SendMessage(removeHoverInfoMsg);
            MessageFactory.CacheMessage(removeHoverInfoMsg);
            var removeHoverEquippedItemMsg = MessageFactory.GenerateRemoveHoveredEquippedItemMsg();
            removeHoverEquippedItemMsg.Controller = this;
            gameObject.SendMessage(removeHoverEquippedItemMsg);
            MessageFactory.CacheMessage(removeHoverEquippedItemMsg);
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