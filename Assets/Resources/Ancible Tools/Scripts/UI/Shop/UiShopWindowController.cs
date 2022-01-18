using System.Collections.Generic;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.Items;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.UI.Alerts;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;
using DataController = Assets.Ancible_Tools.Scripts.System.DataController;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Shop
{
    public class UiShopWindowController : UiBaseWindow
    {
        public static string ObjectId { get; private set; }

        [SerializeField] private RectTransform _content;
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private UiShopItemController _shopItemTemplate;

        private List<UiShopItemController> _controllers = new List<UiShopItemController>();

        private UiShopItemController _hovered = null;

        private string _objectId = string.Empty;

        public void Setup(ShopItemData[] shopItems, string objectId)
        {
            _objectId = objectId;
            for (var i = 0; i < _controllers.Count; i++)
            {
                Destroy(_controllers[i].gameObject);
            }
            _controllers.Clear();
            for (var i = 0; i < shopItems.Length; i++)
            {
                var controller = Instantiate(_shopItemTemplate, _grid.transform);
                controller.Setup(shopItems[i]);
                _controllers.Add(controller);
            }

            var rows = _controllers.Count / _grid.constraintCount;
            var rowCheck = rows * _grid.constraintCount;
            if (rowCheck < _controllers.Count)
            {
                rows++;
            }

            var height = rows * (_grid.cellSize.y + _grid.spacing.y) + _grid.padding.top;
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            DataController.SetShopState(true);
            ObjectId = _objectId;
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<SetHoveredShopItemMessage>(SetHoveredShopItem);
            gameObject.Subscribe<RemoveHoveredShopItemMessage>(RemoveHoveredShopItem);
            gameObject.Subscribe<UpdateInputStateMessage>(UpdateInputState);
            gameObject.Subscribe<ClientFinishInteractionResultMessage>(ClientFinishInteractionResult);
        }

        private void SetHoveredShopItem(SetHoveredShopItemMessage msg)
        {
            _hovered = msg.Controller;
        }

        private void RemoveHoveredShopItem(RemoveHoveredShopItemMessage msg)
        {
            if (_hovered && _hovered == msg.Controller)
            {
                _hovered = null;
            }
        }

        private void UpdateInputState(UpdateInputStateMessage msg)
        {
            if (_hovered && msg.Previous.MouseRight && !msg.Current.MouseRight)
            {
                if (DataController.ActiveCharacter.Gold >= _hovered.ShopItem.Cost)
                {
                    var showShopTransactionMsg = MessageFactory.GenerateShowShopTransactionMsg();
                    showShopTransactionMsg.Item = _hovered.Item;
                    showShopTransactionMsg.ItemId = _hovered.ShopItem.ShopId;
                    showShopTransactionMsg.ObjectId = _objectId;
                    showShopTransactionMsg.Cost = _hovered.ShopItem.Cost;
                    showShopTransactionMsg.Stack = _hovered.ShopItem.Stack;
                    showShopTransactionMsg.Type = ShopTransactionType.Buy;
                    gameObject.SendMessage(showShopTransactionMsg);
                    MessageFactory.CacheMessage(showShopTransactionMsg);
                }
                else
                {
                    UiAlertManager.ShowAlert("Not enough gold");
                }

            }
        }

        private void ClientFinishInteractionResult(ClientFinishInteractionResultMessage msg)
        {
            Close();
        }

        public override void Close()
        {
            ObjectId = string.Empty;
            ClientController.SendMessageToServer(new ClientFinishInteractionRequestMessage());
            DataController.SetShopState(false);
            base.Close();
        }
    }
}