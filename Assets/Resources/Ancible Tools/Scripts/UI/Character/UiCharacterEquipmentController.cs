using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.Items;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.System.CharacterClasses;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Character
{
    public class UiCharacterEquipmentController : MonoBehaviour
    {
        [SerializeField] private List<UiEquippedItemController> _childControllers = new List<UiEquippedItemController>();
        [SerializeField] private UiFillBarController _healthBarController = null;
        [SerializeField] private UiFillBarController _resourceBarController = null;
        [SerializeField] private UiFillBarController _experienceBarController = null;
        [SerializeField] private Image _playerIconImage = null;
        [SerializeField] private Text _classText;
        [SerializeField] private Text _levelText;

        private Dictionary<EquippableSlot, UiEquippedItemController> _controllers = new Dictionary<EquippableSlot, UiEquippedItemController>();

        private UiEquippedItemController _hovered = null;

        void Awake()
        {
            for (var i = 0; i < _childControllers.Count; i++)
            {
                if (!_controllers.ContainsKey(_childControllers[i].Slot))
                {
                    _controllers.Add(_childControllers[i].Slot, _childControllers[i]);
                }
            }
            Refresh();
            SubscribeToMessages();
        }

        private void Refresh()
        {
            var equippedItems = DataController.ActiveCharacter.Equipment.ToList();
            var unequippedItemSlots = _controllers.Where(c => !equippedItems.Exists(e => e.Slot == c.Value.Slot)).Select(kv => kv.Key).ToArray();
            for (var i = 0; i < equippedItems.Count; i++)
            {
                _controllers[equippedItems[i].Slot].Setup(equippedItems[i]);
            }

            for (var i = 0; i < unequippedItemSlots.Length; i++)
            {
                _controllers[unequippedItemSlots[i]].Clear();
            }

            _healthBarController.Setup(DataController.ActiveCharacter.CurrentHealth, DataController.ActiveCharacter.BaseStats.Health + DataController.ActiveCharacter.BonusStats.Health);
            var playerSprite = TraitFactoryController.GetSpriteTraitByName(DataController.ActiveCharacter.Sprite);
            if (playerSprite)
            {
                _playerIconImage.sprite = playerSprite.Icon;
                _playerIconImage.gameObject.SetActive(true);
            }
            else
            {
                _playerIconImage.gameObject.SetActive(false);
            }

            var playerClass = CharacterClassFactoryController.GetClassByName(DataController.ActiveCharacter.PlayerClass);
            if (playerClass)
            {
                _classText.text = $"{playerClass.DisplayName}";
            }
            _levelText.text = $"Level {DataController.ActiveCharacter.Level + 1}";

            _experienceBarController.Setup(DataController.ActiveCharacter.Experience, DataController.ActiveCharacter.NextLevelExperience, $" XP");
            var resource = DataController.ActiveCharacter.Resources[0];
            switch (resource.Resource)
            {
                case AncibleCoreCommon.CommonData.Ability.ResourceType.Spirit:
                    _resourceBarController.SetColor(ColorFactoryController.Spirit);
                    break;
                case AncibleCoreCommon.CommonData.Ability.ResourceType.Mana:
                    _resourceBarController.SetColor(ColorFactoryController.Mana);
                    break;
                case AncibleCoreCommon.CommonData.Ability.ResourceType.Focus:
                    _resourceBarController.SetColor(ColorFactoryController.Focus);
                    break;
            }
            _resourceBarController.Setup(resource.Current, resource.Maximum + resource.Bonus);
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<RefreshPlayerDataMessage>(RefreshPlayerData);
            gameObject.Subscribe<SetHoveredEquippedItemMessage>(SetHoveredEquippedItem);
            gameObject.Subscribe<RemoveHoveredEquippedItemMessage>(RemoveHoveredEquippedItem);
            gameObject.Subscribe<UpdateInputStateMessage>(UpdateInputState);
        }

        private void SetHoveredEquippedItem(SetHoveredEquippedItemMessage msg)
        {
            _hovered = msg.Controller;
        }

        private void RemoveHoveredEquippedItem(RemoveHoveredEquippedItemMessage msg)
        {
            if (_hovered && _hovered == msg.Controller)
            {
                _hovered = null;
            }
        }

        private void RefreshPlayerData(RefreshPlayerDataMessage msg)
        {
            Refresh();
        }

        private void UpdateInputState(UpdateInputStateMessage msg)
        {
            if (_hovered && msg.Previous.MouseRight && !msg.Current.MouseRight)
            {
                if (_hovered.Item)
                {
                    ClientController.SendMessageToServer(new ClientUnEquipItemFromSlotMessage{Slot = _hovered.Slot});
                    GlobalCooldownController.TriggerGlobalCooldown();
                }
            }
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}