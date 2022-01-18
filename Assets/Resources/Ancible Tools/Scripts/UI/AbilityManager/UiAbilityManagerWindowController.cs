using System.Collections.Generic;
using System.Linq;
using AncibleCoreCommon.CommonData.Ability;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Ancible_Tools.Scripts.System.WorldSelect;
using Assets.Resources.Ancible_Tools.Scripts.UI.ActionBar;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.AbilityManager
{
    public class UiAbilityManagerWindowController : UiBaseWindow
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private UiAbilityController _abilityTemplate;

        private Dictionary<string, UiAbilityController> _controllers = new Dictionary<string, UiAbilityController>();

        private UiAbilityController _hovered = null;

        void Awake()
        {
            Refresh();
            SubscribeToMessages();
        }

        private void Refresh()
        {
            var abilities = DataController.ActiveCharacter.Abilities;
            for (var i = 0; i < abilities.Length; i++)
            {
                var ability = abilities[i];
                if (!_controllers.ContainsKey(ability.Name))
                {
                    var controller = Instantiate(_abilityTemplate, _grid.transform);
                    controller.Setup(ability);
                    _controllers.Add(ability.Name, controller);
                }
            }

            var rows = _controllers.Count / _grid.constraintCount;
            var rowCheck = rows * _grid.constraintCount;
            if (rowCheck < _controllers.Count)
            {
                rows++;
            }

            var height = (rows * (_grid.cellSize.y + _grid.spacing.y) + _grid.padding.top);
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            var orderedControllers = _controllers.Values.OrderBy(c => c.Ability.DisplayName).ToArray();
            for (var i = 0; i < orderedControllers.Length; i++)
            {
                orderedControllers[i].transform.SetSiblingIndex(i);
            }
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<RefreshPlayerDataMessage>(RefreshPlayerData);
            gameObject.Subscribe<SetHoveredAbilityMessage>(SetHoveredAbility);
            gameObject.Subscribe<RemoveHoveredAbilityMessage>(RemoveHoveredAbility);
            gameObject.Subscribe<UpdateInputStateMessage>(UpdateInputState);
        }

        private void RefreshPlayerData(RefreshPlayerDataMessage msg)
        {
            Refresh();
        }

        private void SetHoveredAbility(SetHoveredAbilityMessage msg)
        {
            _hovered = msg.Controller;
        }

        private void RemoveHoveredAbility(RemoveHoveredAbilityMessage msg)
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
                    if (_hovered.Ability.TargetType == TargetType.Position)
                    {
                        WorldSelectController.SetupPositionAbility(_hovered.Ability);
                    }
                    else
                    {
                        WorldSelectController.UseAbilityOnSelected(_hovered.Ability);
                    }
                    
                }
                else if (!UiActionBarManagerWindowController.ShortcutActive && msg.Previous.MouseLeft && msg.Current.MouseLeft)
                {
                    UiActionBarManagerWindowController.SetActionShortcut(_hovered.Ability.name, string.Empty, _hovered.Ability.Icon, ActionItemType.Ability);
                }
                
            }
        }
        
        
    }
}