using AncibleCoreCommon;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Server.Items;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Shop
{
    public class UiShopTransactionWindowController : UiBaseWindow
    {
        public override bool Movable => false;
        public override bool Blocking => true;

        [Header("Settings")]
        [SerializeField] private Color _buyColor = Color.white;
        [SerializeField] private Color _sellColor = Color.white;

        [Header("Child References")]
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _itemNameText;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private Text _stackText;
        [SerializeField] private Text _goldPerItemText;
        [SerializeField] private InputField _stackInputText;
        [SerializeField] private Text _totalGoldText;
        [SerializeField] private Text _availableGoldText;
        [SerializeField] private Button _processButton;
        [SerializeField] private Image _borderImage;

        private string _itemId = string.Empty;
        private string _objectId = string.Empty;

        private int _maxStack = 0;
        private int _inputStack = 0;
        private int _cost = 0;

        private ShopTransactionType _type = ShopTransactionType.Buy;

        void Awake()
        {
            SubscribeToMessages();
        }

        public void SetupSell(Item item, string itemId, string objectId, int stack)
        {
            _type = ShopTransactionType.Sell;
            _objectId = objectId;
            _itemId = itemId;
            _titleText.text = "Sell";
            _itemNameText.text = $"{item.DisplayName}";
            _itemIcon.sprite = item.Icon;
            _stackText.text = stack > 1 ? $"{stack}" : string.Empty;
            _maxStack = stack;
            _inputStack = _maxStack;
            _cost = item.SellValue;
            _stackInputText.SetTextWithoutNotify($"{_inputStack}");
            _goldPerItemText.text = $"{item.SellValue}";
            _borderImage.color = _sellColor;
            _totalGoldText.text = $"{_inputStack * _cost}";
            _availableGoldText.text = $"{DataController.ActiveCharacter.Gold}";
        }

        public void SetupBuy(Item item, string itemId, string objectId, int stack, int cost)
        {
            _type = ShopTransactionType.Buy;
            _objectId = objectId;
            _itemId = itemId;
            _titleText.text = "Buy";
            _itemNameText.text = $"{item.DisplayName}";
            _itemIcon.sprite = item.Icon;
            _stackText.text = stack > 1 ? $"{stack}" : string.Empty;
            _cost = cost;
            _maxStack = DataController.ActiveCharacter.Gold / _cost;
            _inputStack = 1;
            _stackInputText.text = $"{_inputStack}";
            _borderImage.color = _buyColor;
            _totalGoldText.text = $"{_inputStack * _cost}";
            _availableGoldText.text = $"{DataController.ActiveCharacter.Gold}";
        }

        public void StackTextUpdated()
        {
            if (int.TryParse(_stackInputText.text, out var stack))
            {
                if (stack > _maxStack)
                {
                    stack = _maxStack;
                    _stackInputText.SetTextWithoutNotify($"{stack}");
                }
                else if (stack < 0)
                {
                    stack = 0;
                    _stackInputText.SetTextWithoutNotify($"{stack}");
                }
                _inputStack = stack;
            }
            else
            {
                _inputStack = 0;
                _stackInputText.SetTextWithoutNotify($"{_inputStack}");
            }

            _totalGoldText.text = $"{_inputStack * _cost}";
            _processButton.interactable = _inputStack > 0;
        }

        public void Process()
        {
            UiServerStatusTextController.SetText("Processing transaction...");
            switch (_type)
            {
                case ShopTransactionType.Buy:
                    ClientController.SendMessageToServer(new ClientBuyItemFromShopRequestMessage{ObjectId = _objectId, ShopId = _itemId, Stack = _inputStack});
                    break;
                case ShopTransactionType.Sell:
                    ClientController.SendMessageToServer(new ClientSellItemToShopRequestMessage{ObjectId = _objectId, ItemId = _itemId, Stack = _inputStack});
                    break;
            }
            Close();
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<ClientFinishInteractionResultMessage>(ClientFinishdInteractionResult);
        }

        private void ClientFinishdInteractionResult(ClientFinishInteractionResultMessage msg)
        {
            Close();
        }
    }
}