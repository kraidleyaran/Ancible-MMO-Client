using AncibleCoreCommon.CommonData.Client;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Server.Items;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MessageFactory = Assets.Ancible_Tools.Scripts.System.MessageFactory;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Inventory
{
    public class UiInventoryItemController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _itemImage;
        [SerializeField] private Image _frameImage;
        [SerializeField] private Text _stackText;
        [SerializeField] private UiCooldownController _cooldownController;
        [SerializeField] private Image _emptySlotImage;

        public ClientItemData Data { get; private set; }
        public Item Item { get; private set; }
        public int Slot { get; private set; }

        private bool _hovered = false;

        void Awake()
        {
            _cooldownController.WakeUp();
        }

        public void Setup(ClientItemData data)
        {
            Data = data;
            Slot = data.Slot;
            var item = ItemFactoryController.GetItemByName(data.Item);
            if (item)
            {
                _cooldownController.SetupGlobalCooldown();
                Item = item;
                _itemImage.sprite = item.Icon;
                _stackText.text = item.MaxStack > 1 ? $"{Data.Stack}" : string.Empty;
                _itemImage.gameObject.SetActive(true);
                _stackText.gameObject.SetActive(true);
                _emptySlotImage.gameObject.SetActive(false);
                if (_hovered)
                {
                    var removeHoveredInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
                    removeHoveredInfoMsg.Owner = gameObject;
                    gameObject.SendMessage(removeHoveredInfoMsg);
                    MessageFactory.CacheMessage(removeHoveredInfoMsg);

                    var setItemHoverInfoMsg = MessageFactory.GenerateSetItemHoverInfoMsg();
                    setItemHoverInfoMsg.Item = Item;
                    setItemHoverInfoMsg.Stack = Data.Stack;
                    setItemHoverInfoMsg.Owner = gameObject;
                    gameObject.SendMessage(setItemHoverInfoMsg);
                    MessageFactory.CacheMessage(setItemHoverInfoMsg);
                }
            }
            else
            {
                Clear();

            }

        }

        public void SetSlot(int slot)
        {
            Slot = slot;
        }

        public void Clear()
        {
            if (_hovered)
            {
                var removeHoveredInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
                removeHoveredInfoMsg.Owner = gameObject;
                gameObject.SendMessage(removeHoveredInfoMsg);
                MessageFactory.CacheMessage(removeHoveredInfoMsg);
            }

            Item = null;
            _itemImage.sprite = null;
            Data = null;
            _stackText.text = string.Empty;
            _itemImage.gameObject.SetActive(false);
            _cooldownController.Clear();
            _emptySlotImage.gameObject.SetActive(true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
            _frameImage.color = UiController.HoverColor;
            if (Item)
            {
                var setItemHoverInfoMsg = MessageFactory.GenerateSetItemHoverInfoMsg();
                setItemHoverInfoMsg.Item = Item;
                setItemHoverInfoMsg.Stack = Data.Stack;
                setItemHoverInfoMsg.Owner = gameObject;
                gameObject.SendMessage(setItemHoverInfoMsg);
                MessageFactory.CacheMessage(setItemHoverInfoMsg);
            }
            var setHoveredInventoryItemMsg = MessageFactory.GenerateSetHoveredInventoryItemMsg();
            setHoveredInventoryItemMsg.Controller = this;
            gameObject.SendMessage(setHoveredInventoryItemMsg);
            MessageFactory.CacheMessage(setHoveredInventoryItemMsg);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
            _frameImage.color = Color.white;
            if (Item)
            {
                var removeHoveredInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
                removeHoveredInfoMsg.Owner = gameObject;
                gameObject.SendMessage(removeHoveredInfoMsg);
                MessageFactory.CacheMessage(removeHoveredInfoMsg);
            }
            var removeHoveredInventoryItemMsg = MessageFactory.GenerateRemoveHoveredInventoryItemMsg();
            removeHoveredInventoryItemMsg.Controller = this;
            gameObject.SendMessage(removeHoveredInventoryItemMsg);
            MessageFactory.CacheMessage(removeHoveredInventoryItemMsg);
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