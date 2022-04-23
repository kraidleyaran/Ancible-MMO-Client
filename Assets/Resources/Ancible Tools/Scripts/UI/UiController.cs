using AncibleCoreCommon;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Ancible_Tools.Scripts.System.SystemMenu;
using Assets.Resources.Ancible_Tools.Scripts.System.CharacterClasses;
using Assets.Resources.Ancible_Tools.Scripts.UI.AbilityManager;
using Assets.Resources.Ancible_Tools.Scripts.UI.Character;
using Assets.Resources.Ancible_Tools.Scripts.UI.Dialogue;
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
        [SerializeField] private UiDialogueWindowController _dialogueTemplate;
        [SerializeField] private UiSystemMenuWindow _systemMenuTemplate;

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
            gameObject.Subscribe<LeaveWorldMessage>(LeaveWorld);

            gameObject.Subscribe<UpdateInputStateMessage>(UpdateInputState);
            gameObject.Subscribe<ShowShopTransactionMessage>(ShowShopTransaction);
            gameObject.Subscribe<ShowDialogueMessage>(ShowDialogue);

            gameObject.Subscribe<ClientShowShopMessage>(ClientShowShop);
            gameObject.Subscribe<ClientShowLootWindowMessage>(ClientShowLoot);
            gameObject.Subscribe<ClientShowDialogueMessage>(ClientShowDialogue);
        }

        private void UpdateInputState(UpdateInputStateMessage msg)
        {
            if (DataController.WorldState == WorldState.Active)
            {
                if (!ActiveInput && !UiWindowManager.IsWindowOpen(_systemMenuTemplate))
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

                    if (msg.Previous.Talents && !msg.Current.Talents)
                    {
                        var characterClass = CharacterClassFactoryController.GetClassByName(DataController.ActiveCharacter.PlayerClass);
                        if (characterClass && characterClass.UiTalentTree)
                        {
                            UiWindowManager.ToggleWindow(characterClass.UiTalentTree);
                        }
                    }
                }

                if (msg.Previous.Escape && !msg.Current.Escape)
                {
                    if (UiWindowManager.IsAnyWindowOpen())
                    {
                        UiWindowManager.CloseAllWindows();
                    }
                    else
                    {
                        UiWindowManager.OpenWindow(_systemMenuTemplate);
                    }
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

        private void ClientShowDialogue(ClientShowDialogueMessage msg)
        {
            var dialogueTrait = TraitFactoryController.GetDialogueByTraitName(msg.Dialogue);
            if (dialogueTrait)
            {
                var dialogueWindow = UiWindowManager.OpenWindow(_dialogueTemplate);
                dialogueWindow.Setup(dialogueTrait.Dialogue, msg.OwnerId);
            }
        }

        private void ShowDialogue(ShowDialogueMessage msg)
        {
            var dialogueWindow = UiWindowManager.OpenWindow(_dialogueTemplate);
            dialogueWindow.Setup(msg.Data, dialogueWindow.OwnerId);
        }

        private void LeaveWorld(LeaveWorldMessage msg)
        {
            UiWindowManager.CloseAllWindows();
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}