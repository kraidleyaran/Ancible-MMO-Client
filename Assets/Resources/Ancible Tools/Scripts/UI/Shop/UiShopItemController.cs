using AncibleCoreCommon.CommonData.Items;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Server.Items;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MessageFactory = Assets.Ancible_Tools.Scripts.System.MessageFactory;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Shop
{
    public class UiShopItemController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Text _stackText;

        public ShopItemData ShopItem { get; private set; }

        public Item Item { get; private set; }

        private bool _hovered = false;

        public void Setup(ShopItemData data)
        {
            ShopItem = data;
            var item = ItemFactoryController.GetItemByName(ShopItem.Item);
            if (item)
            {
                Item = item;
                _iconImage.sprite = item.Icon;
                _stackText.text = $"{data.Stack}";
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
            var setHoveredShopItemMsg = MessageFactory.GenerateSetHoveredShopItemMsg();
            setHoveredShopItemMsg.Controller = this;
            gameObject.SendMessage(setHoveredShopItemMsg);
            MessageFactory.CacheMessage(setHoveredShopItemMsg);

            var setHoveredShopInfoMsg = MessageFactory.GenerateSetShopItemHoverInfoMsg();
            setHoveredShopInfoMsg.ShopItem = Item;
            setHoveredShopInfoMsg.Cost = ShopItem.Cost;
            setHoveredShopInfoMsg.Stack = ShopItem.Stack;
            setHoveredShopInfoMsg.Owner = gameObject;
            gameObject.SendMessage(setHoveredShopInfoMsg);
            MessageFactory.CacheMessage(setHoveredShopInfoMsg);
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = true;
            var removeHoveredShopItemMsg = MessageFactory.GenerateRemoveHoveredShopItemMsg();
            removeHoveredShopItemMsg.Controller = this;
            gameObject.SendMessage(removeHoveredShopItemMsg);
            MessageFactory.CacheMessage(removeHoveredShopItemMsg);

            var removeHoverInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
            removeHoverInfoMsg.Owner = gameObject;
            gameObject.SendMessage(removeHoverInfoMsg);
            MessageFactory.CacheMessage(removeHoverInfoMsg);
        }

        void OnDestroy()
        {
            var removeHoverInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
            removeHoverInfoMsg.Owner = gameObject;
            gameObject.SendMessage(removeHoverInfoMsg);
            MessageFactory.CacheMessage(removeHoverInfoMsg);
        }
    }
}