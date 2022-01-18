using System;
using System.Linq;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.Ability;
using AncibleCoreCommon.CommonData.Client;
using AncibleCoreCommon.CommonData.Items;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Ancible_Tools.Scripts.System.WorldSelect;
using Assets.Resources.Ancible_Tools.Scripts.Server.Abilities;
using Assets.Resources.Ancible_Tools.Scripts.Server.Items;
using Assets.Resources.Ancible_Tools.Scripts.System.Player;
using Assets.Resources.Ancible_Tools.Scripts.UI.Alerts;
using MessageBusLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.ActionBar
{
    public class UiActionBarItemController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _spriteIcon;
        [SerializeField] private Image _frameImage;
        [SerializeField] private Image _emptyImage;
        [SerializeField] private Text _inputKeyText;
        [SerializeField] private UiCooldownController _cooldownController;

        public int Slot { get; private set; }
        public ActionItemType Type { get; private set; } = ActionItemType.Empty;
        public string Name { get; private set; } = string.Empty;
        public Sprite Icon => _spriteIcon.sprite;

        public string Id { get; private set; } = string.Empty;

        private Action _onClick = null;
        

        private bool _hovered = false;

        public void WakeUp()
        {
            _cooldownController.WakeUp();
        }

        public void SetupItem(string itemName, string itemId)
        {
            var item = ItemFactoryController.GetItemByName(itemName);
            if (item)
            {
                _cooldownController.SetupGlobalCooldown();
                Type = ActionItemType.Item;
                Name = itemName;
                Id = itemId;
                _spriteIcon.sprite = item.Icon;
                _spriteIcon.gameObject.SetActive(true);
                _onClick = () => { UseItem(item); };
                _emptyImage.gameObject.SetActive(false);
            }
            else
            {
                Clear();
            }

            
        }

        public void SetupAbility(string abilityName)
        {
            var ability = AbilityFactoryController.GetAbilityFromName(abilityName);
            if (ability)
            {
                if (DataController.ActiveCharacter != null)
                {
                    var clientAbility = DataController.ActiveCharacter.Abilities.FirstOrDefault(a => a.Name == abilityName);
                    _cooldownController.SetupAbility(clientAbility ?? new ClientAbilityInfoData{Name = abilityName, Cooldown = ability.Cooldown, CurrentCooldownTicks = 0});
                }
                else
                {
                    _cooldownController.SetupAbility(new ClientAbilityInfoData { Name = abilityName, Cooldown = ability.Cooldown, CurrentCooldownTicks = 0 });
                }
                Type = ActionItemType.Ability;
                Name = abilityName;
                Id = string.Empty;
                _spriteIcon.sprite = ability.Icon;
                _spriteIcon.gameObject.SetActive(true);
                _onClick = () => { UseAbility(ability); };
                _emptyImage.gameObject.SetActive(false);
            }
            else
            {
                Clear();
            }
        }

        public void Clear()
        {
            _cooldownController.Clear();
            Type = ActionItemType.Empty;
            _onClick = null;
            Id = string.Empty;
            Name = string.Empty;
            _spriteIcon.gameObject.SetActive(false);
            _spriteIcon.sprite = null;
            _emptyImage.gameObject.SetActive(true);
        }

        public void SetSlot(int slot)
        {
            Slot = slot;
        }

        public CharacterActionBarSlot GetPlayerData()
        {
            return new CharacterActionBarSlot
            {
                Name = Name,
                Id = Id,
                Slot = Slot,
                Type = Type
            };
        }

        private void UseAbility(Ability ability)
        {
            if (ability.TargetType == TargetType.Position)
            {
                WorldSelectController.SetupPositionAbility(ability);
            }
            else
            {
                WorldSelectController.UseAbilityOnSelected(ability);
            }
            
        }

        private void UseItem(Item item)
        {
            if (item.Type == ItemType.Useable)
            {
                ClientController.SendMessageToServer(new ClientUseItemMessage { ItemId = Id, Name = Name });
                GlobalCooldownController.TriggerGlobalCooldown();
            }
        }

        public void Click()
        {
            if (_cooldownController.Active)
            {
                UiAlertManager.ShowAlert("On cooldown");
            }
            else
            {
                _onClick?.Invoke();
            }
            
        }

        public void SetInputKey(Key key)
        {
            _inputKeyText.text = $"{key.ToInputString()}";
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
            _frameImage.color = UiController.HoverColor;

            switch (Type)
            {
                case ActionItemType.Ability:
                    var ability = AbilityFactoryController.GetAbilityFromName(Name);
                    if (ability)
                    {
                        if (!WorldSelectController.PositionAbility)
                        {
                            WorldSelectController.SetAbilityArea(ability.Range);
                        }

                        var rank = 0;
                        if (DataController.ActiveCharacter != null)
                        {
                            var existingAbility = DataController.ActiveCharacter.Abilities.FirstOrDefault(a => a.Name == Name);
                            if (existingAbility != null)
                            {
                                rank = existingAbility.Rank;
                            }
                        }
                        var setGeneralHoverInfoMsg = MessageFactory.GenerateSetGeneralHoverInfoMsg();
                        setGeneralHoverInfoMsg.Title = ability.DisplayName;
                        setGeneralHoverInfoMsg.Description = ability.GetClientDescription(rank);
                        setGeneralHoverInfoMsg.Owner = gameObject;
                        setGeneralHoverInfoMsg.Icon = ability.Icon;
                        gameObject.SendMessage(setGeneralHoverInfoMsg);
                        MessageFactory.CacheMessage(setGeneralHoverInfoMsg);
                    }
                    break;
                case ActionItemType.Item:
                    var item = ItemFactoryController.GetItemByName(Name);
                    if (item)
                    {
                        var setGeneralHoverInfoMsg = MessageFactory.GenerateSetGeneralHoverInfoMsg();
                        setGeneralHoverInfoMsg.Title = item.DisplayName;
                        setGeneralHoverInfoMsg.Description = item.GetDescription();
                        setGeneralHoverInfoMsg.Owner = gameObject;
                        setGeneralHoverInfoMsg.Icon = item.Icon;
                        gameObject.SendMessage(setGeneralHoverInfoMsg);
                        MessageFactory.CacheMessage(setGeneralHoverInfoMsg);
                    }
                    break;
            }
            var setHoveredActionBarItemMsg = MessageFactory.GenerateSetHoveredActionBarItemMsg();
            setHoveredActionBarItemMsg.Controller = this;
            gameObject.SendMessage(setHoveredActionBarItemMsg);
            MessageFactory.CacheMessage(setHoveredActionBarItemMsg);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
            _frameImage.color = Color.white;
            switch (Type)
            {
                case ActionItemType.Ability:
                    if (!WorldSelectController.PositionAbility)
                    {
                        WorldSelectController.ClearAbilityArea();
                    }
                    break;
            }

            if (Type != ActionItemType.Empty)
            {
                var removeHoverInfoMsg = MessageFactory.GenerateRemoveHoverInfoMsg();
                removeHoverInfoMsg.Owner = gameObject;
                gameObject.SendMessage(removeHoverInfoMsg);
                MessageFactory.CacheMessage(removeHoverInfoMsg);
            }

            var removeHoveredActionBarItemMsg = MessageFactory.GenerateRemoveHoveredActionBarItemMsg();
            removeHoveredActionBarItemMsg.Controller = this;
            gameObject.SendMessage(removeHoveredActionBarItemMsg);
            MessageFactory.CacheMessage(removeHoveredActionBarItemMsg);
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