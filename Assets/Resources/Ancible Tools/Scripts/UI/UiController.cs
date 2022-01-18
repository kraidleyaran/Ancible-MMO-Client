using AncibleCoreCommon;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.UI.AbilityManager;
using Assets.Resources.Ancible_Tools.Scripts.UI.Character;
using Assets.Resources.Ancible_Tools.Scripts.UI.Inventory;
using Assets.Resources.Ancible_Tools.Scripts.UI.Loot;
using Assets.Resources.Ancible_Tools.Scripts.UI.Shop;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using MessageBusLib;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.UI
{
    public class UiController : MonoBehaviour
    {
        public static Color HoverColor => _instance._globalHoverColor;
        public static Color SelectColor => _instance._globalSelectColor;
        public static GameObject ActiveInput { get; private set; }

        private static UiController _instance = null;

        [Header("Settings")]
        [SerializeField] private Color _globalHoverColor = Color.white;
        [SerializeField] private Color _globalSelectColor = Color.white;

        [Header("Window Prefab References")]
        [SerializeField] private UiInventoryWindowController _inventoryTemplate;
        [SerializeField] private UiCharacterWindowController _characterTemplate;
        [SerializeField] private UiAbilityManagerWindowController _abilityTemplate;
        [SerializeField] private UiShopWindowController _shopWindowTemplate;
        [SerializeField] private UiShopTransactionWindowController _shopTransactionTemplate;
        [SerializeField] private UiLootWindowController _lootWindowTemplate;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            SubscribeToMessages();
        }

        public static void SetActiveInputField(GameObject inputField)
        {
            ActiveInput = inputField;
        }

        public static void RemoveActiveInputField(GameObject inputField)
        {
            if (ActiveInput && ActiveInput == inputField)
            {
                ActiveInput = null;
            }
        }

        private void SubscribeToMessages()
        {

            gameObject.Subscribe<UpdateInputStateMessage>(UpdateInputState);
            gameObject.Subscribe<ShowShopTransactionMessage>(ShowShopTransaction);

            gameObject.Subscribe<ClientShowShopMessage>(ClientShowShop);
            gameObject.Subscribe<ClientShowLootWindowMessage>(ClientShowLoot);

        }

        private void UpdateInputState(UpdateInputStateMessage msg)
        {
            if (DataController.WorldState == WorldState.Active && !ActiveInput)
            {
                if (msg.Previous.Inventory && !msg.Current.Inventory)
                {
                    UiWindowManager.ToggleWindow(_inventoryTemplate);
                }

                if (msg.Previous.Character && !msg.Current.Character)
                {
                    UiWindowManager.ToggleWindow(_characterTemplate);
                }

                if (msg.Previous.Abilities && !msg.Current.Abilities)
                {
                    UiWindowManager.ToggleWindow(_abilityTemplate);
                }
            }
        }

        private void ShowShopTransaction(ShowShopTransactionMessage msg)
        {
            var transactionWindow = UiWindowManager.OpenWindow(_shopTransactionTemplate);
            switch (msg.Type)
            {
                case ShopTransactionType.Buy:
                    transactionWindow.SetupBuy(msg.Item, msg.ItemId, msg.ObjectId, msg.Stack, msg.Cost);
                    break;
                case ShopTransactionType.Sell:
                    transactionWindow.SetupSell(msg.Item, msg.ItemId, msg.ObjectId, msg.Stack);
                    break;
            }
        }

        private void ClientShowShop(ClientShowShopMessage msg)
        {
            var shopWindow = UiWindowManager.OpenWindow(_shopWindowTemplate);
            shopWindow.Setup(msg.ShopItems, msg.ObjectId);
            UiWindowManager.OpenWindow(_inventoryTemplate);
        }

        private void ClientShowLoot(ClientShowLootWindowMessage msg)
        {
            var lootWindow = UiWindowManager.OpenWindow(_lootWindowTemplate);
            lootWindow.Setup(msg.Loot, msg.ObjectId);
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}