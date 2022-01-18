using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.Items;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Server.Items;
using Assets.Resources.Ancible_Tools.Scripts.UI.ActionBar;
using Assets.Resources.Ancible_Tools.Scripts.UI.Shop;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;
using ItemType = AncibleCoreCommon.CommonData.Items.ItemType;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Inventory
{
    public class UiInventoryWindowController : UiBaseWindow
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private UiInventoryItemController _itemTemplate;
        [SerializeField] private Text _goldText;

        private Dictionary<int, UiInventoryItemController> _inventory = new Dictionary<int, UiInventoryItemController>();

        private UiInventoryItemController _hovered = null;

        void Awake()
        {
            SubscribeToMessages();
            Refresh();
        }

        private void Refresh()
        {
            if (_inventory.Count < DataController.ActiveCharacter.MaxInventorySlots)
            {
                var missing = DataController.ActiveCharacter.MaxInventorySlots - _inventory.Count;
                for (var i = 0; i < missing; i++)
                {
                    var controller = Instantiate(_itemTemplate, _content.transform);
                    controller.SetSlot(_inventory.Count);
                    _inventory.Add(_inventory.Count, controller);
                }
            }

            var playerItems = DataController.ActiveCharacter.Inventory.ToArray();
            var emptySlots = _inventory.Keys.Where(s => playerItems.FirstOrDefault(i => i.Slot == s) == null).ToArray();
            for (var i = 0; i < playerItems.Length; i++)
            {
                _inventory[playerItems[i].Slot].Setup(playerItems[i]);
            }

            for (var i = 0; i < emptySlots.Length; i++)
            {
                _inventory[emptySlots[i]].Clear();
            }

            var rows = _inventory.Count / _grid.constraintCount;
            var rowCheckCount = rows * _grid.constraintCount;
            if (rowCheckCount < _inventory.Count)
            {
                rows++;
            }

            var height = rows * (_grid.cellSize.y + _grid.spacing.y) + _grid.padding.top;
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            _goldText.text = $"{DataController.ActiveCharacter.Gold:n0}";
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<UpdateInputStateMessage>(UpdateInputState);
            gameObject.Subscribe<SetHoveredInventoryItemMessage>(SetHoveredInventoryItem);
            gameObject.Subscribe<RemoveHoveredInventoryItemMessage>(RemoveHoveredInventoryItem);
            gameObject.Subscribe<RefreshPlayerDataMessage>(RefreshPlayerData);
        }

        private void SetHoveredInventoryItem(SetHoveredInventoryItemMessage msg)
        {
            _hovered = msg.Controller;
        }

        private void RemoveHoveredInventoryItem(RemoveHoveredInventoryItemMessage msg)
        {
            if (_hovered && _hovered == msg.Controller)
            {
                _hovered = null;
            }
        }

        private void UpdateInputState(UpdateInputStateMessage msg)
        {
            if (!UiWindowManager.Moving && _hovered)
            {
                if (msg.Previous.MouseRight && !msg.Current.MouseRight)
                {
                    if (DataController.ShopOpen)
                    {
                        if (_hovered.Item.SellValue >= 0)
                        {
                            var showShopTransactionMsg = MessageFactory.GenerateShowShopTransactionMsg();
                            showShopTransactionMsg.Type = ShopTransactionType.Sell;
                            showShopTransactionMsg.Item = _hovered.Item;
                            showShopTransactionMsg.ItemId = _hovered.Data.ItemId;
                            showShopTransactionMsg.Stack = _hovered.Data.Stack;
                            showShopTransactionMsg.ObjectId = UiShopWindowController.ObjectId;
                            gameObject.SendMessage(showShopTransactionMsg);
                            MessageFactory.CacheMessage(showShopTransactionMsg);
                        }
                    }
                    else
                    {
                        if (_hovered.Item.Type == ItemType.Useable)
                        {
                            ClientController.SendMessageToServer(new ClientUseItemMessage { ItemId = _hovered.Data.ItemId });
                            GlobalCooldownController.TriggerGlobalCooldown();
                        }
                        else if (_hovered.Item.Type == ItemType.Equippable)
                        {
                            ClientController.SendMessageToServer(new ClientEquipItemMessage { ItemId = _hovered.Data.ItemId });
                            GlobalCooldownController.TriggerGlobalCooldown();
                        }
                    }
                }
                else if (UiActionBarManagerWindowController.ShortcutActive && UiActionBarManagerWindowController.ShortcutController.Type == ActionItemType.Item)
                {
                    if (msg.Previous.MouseLeft && !msg.Current.MouseLeft)
                    {
                        var clientItem = DataController.ActiveCharacter.Inventory.FirstOrDefault(i => i.ItemId == UiActionBarManagerWindowController.ShortcutController.Id);
                        if (clientItem != null && (_hovered.Data == null || clientItem.ItemId != _hovered.Data.ItemId))
                        {
                            ClientController.SendMessageToServer(new ClientMoveItemToSlotRequestMessage{ItemId = clientItem.ItemId, Slot = _hovered.Slot});
                            GlobalCooldownController.TriggerGlobalCooldown();
                        }
                    }
                }
                else if (msg.Previous.MouseLeft && msg.Current.MouseLeft)
                {
                    if (_hovered.Item)
                    {
                        UiActionBarManagerWindowController.SetActionShortcut(_hovered.Item.name, _hovered.Data.ItemId, _hovered.Item.Icon, ActionItemType.Item, -1, _hovered.Item.Type == ItemType.Useable);
                    }
                }
            }
        }

        private void RefreshPlayerData(RefreshPlayerDataMessage msg)
        {
            Refresh();
        }

    }
}