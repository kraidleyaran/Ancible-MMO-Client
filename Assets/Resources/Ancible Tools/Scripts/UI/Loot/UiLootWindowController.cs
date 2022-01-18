using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.Client;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Loot
{
    public class UiLootWindowController : UiBaseWindow
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private UiLootItemController _lootItemTemplate;

        private List<UiLootItemController> _controllers = new List<UiLootItemController>();

        private UiLootItemController _hovered = null;

        private string _objectId = string.Empty;

        void Awake()
        {
            SubscribeToMessages();
        }

        public void Setup(ClientLootItemData[] loot, string objectId)
        {
            _objectId = objectId;
            var removedControllers = _controllers.Where(c => loot.FirstOrDefault(l => l.Id == c.ItemId) == null).ToArray();
            for (var i = 0; i < removedControllers.Length; i++)
            {
                _controllers.Remove(removedControllers[i]);
                Destroy(removedControllers[i].gameObject);
            }

            for (var i = 0; i < loot.Length; i++)
            {
                var controller = _controllers.FirstOrDefault(c => c.ItemId == loot[i].Id);
                if (!controller)
                {
                    controller = Instantiate(_lootItemTemplate, _grid.transform);
                    _controllers.Add(controller);
                }
                controller.Setup(loot[i], objectId);
            }

            var rows = _controllers.Count / _grid.constraintCount;
            var rowCheck = _grid.constraintCount * rows;
            if (rowCheck < _controllers.Count)
            {
                rows++;
            }

            var height = rows * (_grid.spacing.y + _grid.cellSize.y) + _grid.padding.top;
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        public void TakeAll()
        {
            ClientController.SendMessageToServer(new ClientLootAllRequestMessage{ObjectId = _objectId});
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientFinishInteractionResultMessage>(ClientFinishInteractionResult);

            gameObject.Subscribe<SetHoveredLootItemMessage>(SetHoveredLootItem);
            gameObject.Subscribe<RemoveHoveredLootItemMessage>(RemoveHoveredLootItem);
        }

        private void ClientFinishInteractionResult(ClientFinishInteractionResultMessage msg)
        {
            Close();
        }

        private void SetHoveredLootItem(SetHoveredLootItemMessage msg)
        {
            _hovered = msg.Controller;
        }

        private void RemoveHoveredLootItem(RemoveHoveredLootItemMessage msg)
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
                _hovered.OnClick();
            }
        }
    }
}