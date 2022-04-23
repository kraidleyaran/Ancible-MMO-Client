using System;
using System.Linq;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Ancible_Tools.Scripts.System.Input;
using Assets.Resources.Ancible_Tools.Scripts.System.Player;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using MessageBusLib;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.ActionBar
{
    public class UiActionBarManagerWindowController : UiBaseWindow
    {
        public static bool ShortcutActive => _instance._shortcutController.gameObject.activeSelf;
        public static UiActionBarShortcutController ShortcutController => _instance._shortcutController;

        private static UiActionBarManagerWindowController _instance = null;

        public override bool Movable => false;
        public override bool IsChild => false;

        [SerializeField] private UiActionBarItemController[] _controllers;
        [SerializeField] private UiActionBarShortcutController _shortcutController;

        private UiActionBarItemController _hovered = null;
        private Vector2 _cursorPosition = Vector2.zero;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }
            SubscribeToMessages();
            _shortcutController.Clear();
            for (var i = 0; i < _controllers.Length; i++)
            {
                _controllers[i].Clear();
                _controllers[i].SetSlot(i);
                _controllers[i].WakeUp();
            }
            _instance = this;
            gameObject.SetActive(false);
        }

        public static void SetActionShortcut(string name, string id, Sprite icon, ActionItemType type, int existingSlot = -1, bool actionable = true)
        {
            if (!GlobalCooldownController.Active)
            {
                _instance._shortcutController.RectTransform.SetTransformPosition(_instance._cursorPosition);
                _instance._shortcutController.Setup(name, id, icon, type, existingSlot, actionable);
            }
        }

        public static CharacterActionBarSlot[] GetPlayerData()
        {
            return _instance._controllers.Select(c => c.GetPlayerData()).ToArray();
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<UpdateInputStateMessage>(UpdateInputState);
            gameObject.Subscribe<SetHoveredActionBarItemMessage>(SetHoveredActionBarItem);
            gameObject.Subscribe<RemoveHoveredActionBarItemMessage>(RemoveHoveredActionBarItem);
            gameObject.Subscribe<UpdateWorldStateMessage>(UpdateWorldState);
            gameObject.Subscribe<CharacterSettingsLoadedMessage>(CharacterSettingsLoaded);
        }

        private void UpdateInputState(UpdateInputStateMessage msg)
        {
            if (DataController.WorldState == WorldState.Active && !UiWindowManager.Moving)
            {
                _cursorPosition = msg.Current.MousePosition;
                if (_shortcutController.gameObject.activeSelf)
                {
                    if (msg.Current.MouseLeft)
                    {
                        _shortcutController.MoveDelta(msg.Current.MousePosition - msg.Previous.MousePosition);
                    }
                    else
                    {
                        if (_hovered && _shortcutController.Actionable)
                        {
                            switch (_shortcutController.Type)
                            {
                                case ActionItemType.Ability:
                                    if (_shortcutController.ExistingSlot > -1)
                                    {
                                        _hovered.SetupAbility(_shortcutController.Action);
                                        if (_hovered.Type != ActionItemType.Empty)
                                        {
                                            var existingBar = _controllers.FirstOrDefault(c => c.Slot == _shortcutController.ExistingSlot);
                                            if (existingBar)
                                            {
                                                existingBar.Clear();
                                                //switch (hoveredType)
                                                //{
                                                //    case ActionItemType.Ability:
                                                //        existingBar.SetupAbility(hoveredName);
                                                //        break;
                                                //    case ActionItemType.Item:
                                                //        existingBar.SetupItem(hoveredName, hoveredId);
                                                //        break;
                                                //}
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _hovered.SetupAbility(_shortcutController.Action);
                                    }
                                    
                                    break;
                                case ActionItemType.Item:
                                    if (_shortcutController.ExistingSlot > -1)
                                    {
                                        _hovered.SetupItem(_shortcutController.Action, _shortcutController.Id);
                                        if (_hovered.Type != ActionItemType.Empty)
                                        {
                                            var existingBar = _controllers.FirstOrDefault(c => c.Slot == _shortcutController.ExistingSlot);
                                            if (existingBar)
                                            {
                                                existingBar.Clear();
                                                //switch (hoveredType)
                                                //{
                                                //    case ActionItemType.Ability:
                                                //        existingBar.SetupAbility(hoveredName);
                                                //        break;
                                                //    case ActionItemType.Item:
                                                //        existingBar.SetupItem(hoveredName, hoveredId);
                                                //        break;
                                                //}
                                            }
                                        }
                                        //else
                                        //{
                                        //    _hovered.SetupItem(_shortcutController.Action, _shortcutController.Id);
                                        //}
                                    }
                                    else
                                    {
                                        _hovered.SetupItem(_shortcutController.Action, _shortcutController.Id);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            if (_shortcutController.ExistingSlot > -1)
                            {
                                var existing = _controllers.FirstOrDefault(c => c.Slot == _shortcutController.ExistingSlot);
                                if (existing)
                                {
                                    existing.Clear();
                                }
                            }
                            else if (_shortcutController.Type == ActionItemType.Item)
                            {
                                //TODO: If item, destroy from inventory
                            }
                        }

                        StartCoroutine(StaticMethods.WaitForFrames(1, () => { _shortcutController.Clear(); }));

                    }
                }
                else if (_hovered && msg.Previous.MouseLeft && msg.Current.MouseLeft)
                {
                    if (_hovered.Type != ActionItemType.Empty)
                    {
                        SetActionShortcut(_hovered.Name, _hovered.Id, _hovered.Icon, _hovered.Type, _hovered.Slot);
                    }
                }
                else if (_hovered && msg.Previous.MouseRight && !msg.Current.MouseRight)
                {
                    _hovered.Click();
                }
                else if (!UiController.ActiveInput)
                {
                    for (var i = 0; i < msg.Previous.ActionBar.Length; i++)
                    {
                        if (msg.Previous.ActionBar[i] && !msg.Current.ActionBar[i] && _controllers.Length > i)
                        {
                            _controllers[i].Click();
                        }
                    }
                }

            }

        }

        private void SetHoveredActionBarItem(SetHoveredActionBarItemMessage msg)
        {
            _hovered = msg.Controller;
        }

        private void RemoveHoveredActionBarItem(RemoveHoveredActionBarItemMessage msg)
        {
            if (_hovered && _hovered == msg.Controller)
            {
                _hovered = null;
            }
        }

        private void UpdateWorldState(UpdateWorldStateMessage msg)
        {
            gameObject.SetActive(msg.State == WorldState.Active);
        }

        private void CharacterSettingsLoaded(CharacterSettingsLoadedMessage msg)
        {
            var characterSlots = DataController.ActivePlayerSettings.ActionSlots;
            for (var i = 0; i < characterSlots.Length; i++)
            {
                if (_controllers.Length > characterSlots[i].Slot)
                {
                    var slot = characterSlots[i];
                    switch (slot.Type)
                    {
                        case ActionItemType.Empty:
                            _controllers[slot.Slot].Clear();
                            break;
                        case ActionItemType.Ability:
                            var ability = AbilityFactoryController.GetAbilityFromName(slot.Name);
                            if (ability)
                            {
                                _controllers[slot.Slot].SetupAbility(ability.name);
                            }
                            else
                            {
                                _controllers[slot.Slot].Clear();
                            }
                            break;
                        case ActionItemType.Item:
                            var item = ItemFactoryController.GetItemByName(slot.Name);
                            if (item)
                            {
                                _controllers[slot.Slot].SetupItem(slot.Name, slot.Id);
                            }
                            else
                            {
                                _controllers[slot.Slot].Clear();
                            }
                            break;
                    }
                }

            }

            for (var i = 0; i < _controllers.Length; i++)
            {
                if (WorldInputController.ActionBar.Length > i)
                {
                    _controllers[i].SetInputKey(WorldInputController.ActionBar[i]);
                }
            }
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}