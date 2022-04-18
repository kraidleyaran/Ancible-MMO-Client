using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon.CommonData;
using AncibleCoreCommon.CommonData.Client;
using AncibleCoreCommon.CommonData.Combat;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.UI.Nameplate;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.ObjectInfo
{
    public class UiObjectInfoWindowController : UiBaseWindow
    {
        [SerializeField] private Image _objectImage;
        [SerializeField] private Text _objectNameText;
        [SerializeField] private UiFillBarController _healthBarController;
        [SerializeField] private UiInteractionButtonController _interactionButtonTemplate;
        [SerializeField] private GameObject _interactionsGroup;
        [SerializeField] private GridLayoutGroup _iconsGrid;
        [SerializeField] private RectTransform _iconsContent;
        [SerializeField] private GameObject _statusEffectsGroup;
        
        [Header("Interaction Icon Settings")]
        [SerializeField] private Sprite _shopIcon;
        [SerializeField] private Sprite _healerIcon;
        [SerializeField] private Sprite _lootIcon;
        [SerializeField] private Sprite _checkpointIcon;
        [SerializeField] private Sprite _talkIcon;
        [SerializeField] private Sprite _inspectIcon;

        private Dictionary<InteractionType, UiInteractionButtonController> _interactionButtons = new Dictionary<InteractionType, UiInteractionButtonController>();
        private Dictionary<StatusEffectType, UiStatusEffectController> _statusEffects = new Dictionary<StatusEffectType, UiStatusEffectController>();
        private Dictionary<string, UiObjectIconController> _icons = new Dictionary<string, UiObjectIconController>();

        private GameObject _obj = null;

        void Awake()
        {
            SubscribeToMessages();
        }

        public void Setup(GameObject obj)
        {
            _obj = obj;
            Refresh();
        }

        private void Refresh()
        {
            ClientObjectData objData = null;
            var queryObjectDataMsg = MessageFactory.GenerateQueryNetworkObjectDataMsg();
            queryObjectDataMsg.DoAfter = data => { objData = data; };
            gameObject.SendMessageTo(queryObjectDataMsg, _obj);
            MessageFactory.CacheMessage(queryObjectDataMsg);
            if (objData != null)
            {
                var sprite = TraitFactoryController.GetSpriteTraitByName(objData.Sprite);
                if (sprite)
                {
                    _objectImage.sprite = sprite.Icon;
                }

                _objectNameText.text = objData.Name;
                if (objData.MaxHealth > 0)
                {
                    _healthBarController.Setup(objData.Health, objData.MaxHealth);
                }
                else
                {
                    _healthBarController.Setup(1,1);
                }

                for (var i = 0; i < objData.Interactions.Length; i++)
                {
                    if (!_interactionButtons.TryGetValue(objData.Interactions[i], out var button))
                    {
                        button = Instantiate(_interactionButtonTemplate, _interactionsGroup.transform);
                        _interactionButtons.Add(objData.Interactions[i], button);
                    }
                    button.Setup(objData.Interactions[i], objData.ObjectId);
                    switch (objData.Interactions[i])
                    {
                        case InteractionType.Talk:
                            button.SetIcon(_talkIcon);
                            break;
                        case InteractionType.Shop:
                            button.SetIcon(_shopIcon);
                            break;
                        case InteractionType.Party:
                            break;
                        case InteractionType.Heal:
                            button.SetIcon(_healerIcon);
                            break;
                        case InteractionType.Loot:
                            button.SetIcon(_lootIcon);
                            break;
                        case InteractionType.Checkpoint:
                            button.SetIcon(_checkpointIcon);
                            break;
                        case InteractionType.Inspect:
                            button.SetIcon(_inspectIcon);
                            break;
                    }
                }

                var removed = _interactionButtons.Values.Where(b => !objData.Interactions.Contains(b.Interaction)).ToArray();
                for (var i = 0; i < removed.Length; i++)
                {
                    _interactionButtons.Remove(removed[i].Interaction);
                    Destroy(removed[i].gameObject);
                }

                var statusEffects = objData.StatusEffects.ToList();
                var removedEffects = _statusEffects.Keys.Where(e => !statusEffects.Exists(s => s.Type == e)).ToArray();
                for (var i = 0; i < removedEffects.Length; i++)
                {
                    var controller = _statusEffects[removedEffects[i]];
                    _statusEffects.Remove(removedEffects[i]);
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
                    _statusEffectsGroup.gameObject.SetActive(true);
                }
                else
                {
                    ClearStatusEffects();
                }

                var icons = objData.Icons.ToList();
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
                        controller = Instantiate(UiNameplateManager.ObjectIcon, _iconsGrid.transform);
                        _icons.Add(icons[i].Id, controller);
                    }
                    controller.Setup(icons[i]);
                }

                if (_icons.Count > 0)
                {
                    var rows = _icons.Count / _iconsGrid.constraintCount;
                    var rowCheck = rows * _iconsGrid.constraintCount;
                    if (rowCheck < _icons.Count)
                    {
                        rows++;
                    }

                    var height = rows * (_iconsGrid.cellSize.y + _iconsGrid.spacing.y) + _iconsGrid.padding.top;
                    _iconsContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                    _iconsGrid.gameObject.SetActive(true);
                }
                else
                {
                    ClearIcons();
                }
            }
            else
            {
                Close();
            }
        }

        private void ClearInteractions()
        {
            var buttons = _interactionButtons.Values.ToArray();
            for (var i = 0; i < buttons.Length; i++)
            {
                Destroy(buttons[i].gameObject);
            }
            _interactionButtons.Clear();
        }

        private void ClearIcons()
        {
            _iconsContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,0f);
            _iconsGrid.gameObject.SetActive(false);
        }

        private void ClearStatusEffects()
        {
            _statusEffectsGroup.gameObject.SetActive(false);
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<RefreshWorldDataMessage>(RefreshWorldData);
        }

        private void RefreshWorldData(RefreshWorldDataMessage msg)
        {
            Refresh();
        }
    }
}