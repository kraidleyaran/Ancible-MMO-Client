using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.Combat;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.UI.Nameplate;
using Assets.Resources.Ancible_Tools.Scripts.UI.ObjectInfo;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.PlayerOverlay
{
    public class UiPlayerOverlayController : MonoBehaviour
    {

        [Header("Child References")]
        [SerializeField] private Image _characterIconImage;
        [SerializeField] private UiFillBarController _healthBar;
        [SerializeField] private UiFillBarController _resourceBar;
        [SerializeField] private Image _resourceIconImage;
        [SerializeField] private GameObject _statusEffectsGroup;
        [SerializeField] private GridLayoutGroup _iconGroup;
        [SerializeField] private RectTransform _iconGroupContent;
        [SerializeField] private Text _levelText;

        private Dictionary<StatusEffectType, UiStatusEffectController> _statusEffects = new Dictionary<StatusEffectType, UiStatusEffectController>();
        private Dictionary<string, UiObjectIconController> _icons = new Dictionary<string, UiObjectIconController>();

        void Awake()
        {
            SubscribeToMessages();
            gameObject.SetActive(false);
        }

        private void Refresh()
        {
            if (DataController.ActiveCharacter != null)
            {
                var resource = DataController.ActiveCharacter.Resources[0];
                if (gameObject.activeSelf)
                {

                    _healthBar.DoFill(DataController.ActiveCharacter.CurrentHealth, DataController.ActiveCharacter.BaseStats.Health + DataController.ActiveCharacter.BonusStats.Health);
                    _resourceBar.DoFill(resource.Current, resource.Maximum + resource.Bonus);
                }
                else
                {
                    _healthBar.Setup(DataController.ActiveCharacter.CurrentHealth, DataController.ActiveCharacter.BaseStats.Health + DataController.ActiveCharacter.BonusStats.Health);
                    _resourceBar.Setup(resource.Current, resource.Maximum + resource.Bonus);
                }

                var statusEffects = DataController.ActiveCharacter.StatusEffects.ToList();
                var removed = _statusEffects.Keys.Where(e => !statusEffects.Exists(s => s.Type == e)).ToArray();
                for (var i = 0; i < removed.Length; i++)
                {
                    var controller = _statusEffects[removed[i]];
                    _statusEffects.Remove(removed[i]);
                    Destroy(controller.gameObject);
                }

                for (var i = 0; i < statusEffects.Count; i++)
                {
                    if (!_statusEffects.TryGetValue(statusEffects[i].Type, out var controller))
                    {
                        controller = Instantiate(UiNameplateManager.StatusEffect, _statusEffectsGroup.transform);
                        _statusEffects.Add(statusEffects[i].Type, controller);
                    }
                    controller.Setup(statusEffects[i], gameObject);
                }

                if (_statusEffects.Count > 0)
                {
                    var orderedEffects = _statusEffects.Values.OrderBy(s => s.StatusEffect).ToArray();
                    for (var i = 0; i < orderedEffects.Length; i++)
                    {
                        orderedEffects[i].transform.SetSiblingIndex(i);
                    }
                }
                else
                {
                    ClearStatusEffects();
                }

                _levelText.text = $"{DataController.ActiveCharacter.Level + 1}";

                var icons = DataController.ActiveCharacter.Icons.ToList();
                var removedIcons = _icons.Values.Where(i => !icons.Exists(c => c.Id == i.Id)).ToArray();
                for (var i = 0; i < removedIcons.Length; i++)
                {
                    _icons.Remove(removedIcons[i].Id);
                    Destroy(removedIcons[i].gameObject);
                }

                for (var i = 0; i < icons.Count; i++)
                {
                    if (!_icons.TryGetValue(icons[i].Id, out var controller))
                    {
                        controller = Instantiate(UiNameplateManager.ObjectIcon, _iconGroup.transform);
                        _icons.Add(icons[i].Id, controller);
                    }
                    controller.Setup(icons[i]);
                }

                if (_icons.Count > 0)
                {
                    var rows = _icons.Count / _iconGroup.constraintCount;
                    var rowCheck = rows * _iconGroup.constraintCount;
                    if (rowCheck < _icons.Count)
                    {
                        rows++;
                    }

                    var height = rows * (_iconGroup.cellSize.y + _iconGroup.spacing.y) + _iconGroup.padding.bottom;
                    _iconGroupContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                    _iconGroup.gameObject.SetActive(true);
                }
                else
                {
                    ClearIcons();
                }
            }

        }

        private void ClearStatusEffects()
        {
            var keys = _statusEffects.Keys.ToArray();
            for (var i = 0; i < keys.Length; i++)
            {
                var controller = _statusEffects[keys[i]];
                Destroy(controller.gameObject);
            }
            _statusEffects.Clear();
            _statusEffects = new Dictionary<StatusEffectType, UiStatusEffectController>();
            _statusEffectsGroup.gameObject.SetActive(false);
        }

        private void ClearIcons()
        {
            _iconGroupContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
            _iconGroup.gameObject.SetActive(false);
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<RefreshPlayerDataMessage>(RefreshPlayerData);
            gameObject.Subscribe<UpdateWorldStateMessage>(UpdateWorldState);
            gameObject.Subscribe<ClientEnterWorldWithCharacterResultMessage>(ClientEnterWorldWithCharacter);
        }

        private void RefreshPlayerData(RefreshPlayerDataMessage msg)
        {
            Refresh();
        }

        private void UpdateWorldState(UpdateWorldStateMessage msg)
        {
            if (msg.State == WorldState.Active)
            {
                if (gameObject.activeSelf)
                {
                    Refresh();
                }
                else
                {
                    Refresh();
                    gameObject.SetActive(true);
                }
            }
            else if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }

        private void ClientEnterWorldWithCharacter(ClientEnterWorldWithCharacterResultMessage msg)
        {
            if (msg.Success)
            {
                var playerSprite = TraitFactoryController.GetSpriteTraitByName(msg.Data.Sprite);
                if (playerSprite)
                {
                    _characterIconImage.sprite = playerSprite.Icon;
                    _characterIconImage.gameObject.SetActive(true);
                }
                else
                {
                    _characterIconImage.gameObject.SetActive(false);
                }

                _healthBar.Setup(msg.Data.CurrentHealth, msg.Data.BaseStats.Health + msg.Data.BonusStats.Health);
                var resource = msg.Data.Resources[0];
                _resourceBar.Setup(resource.Current, resource.Maximum + resource.Bonus);
                switch (resource.Resource)
                {
                    case AncibleCoreCommon.CommonData.Ability.ResourceType.Spirit:
                        _resourceBar.SetColor(ColorFactoryController.Spirit);
                        _resourceIconImage.color = ColorFactoryController.Spirit;
                        _resourceIconImage.sprite = AbilityFactoryController.Spirit;
                        break;
                    case AncibleCoreCommon.CommonData.Ability.ResourceType.Mana:
                        _resourceBar.SetColor(ColorFactoryController.Mana);
                        _resourceIconImage.color = ColorFactoryController.Mana;
                        _resourceIconImage.sprite = AbilityFactoryController.Mana;
                        break;
                    case AncibleCoreCommon.CommonData.Ability.ResourceType.Focus:
                        _resourceBar.SetColor(ColorFactoryController.Focus);
                        _resourceIconImage.color = ColorFactoryController.Focus;
                        _resourceIconImage.sprite = AbilityFactoryController.Focus;
                        break;
                }
            }
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }

}