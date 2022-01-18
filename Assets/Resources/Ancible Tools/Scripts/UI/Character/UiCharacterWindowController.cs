using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.UI.Windows;
using MessageBusLib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Character
{
    public class UiCharacterWindowController : UiBaseWindow
    {
        [Header("Child References")]
        [SerializeField] private RectTransform _content;
        [SerializeField] private Text _titleText;

        [Header("Prefab References")]
        [SerializeField] private UiCharacterEquipmentController _characterEquipmentTemplate;
        [SerializeField] private UiCharacterCombatStatsController _characterStatsTemplate;

        private GameObject _currentTab = null;

        private CharacterWindowState _windowState = CharacterWindowState.Equipment;

        void Awake()
        {
            _currentTab = Instantiate(_characterEquipmentTemplate, _content).gameObject;
            _titleText.text = DataController.ActiveCharacter.Name;
        }

        public void EquipmentState()
        {
            if (_windowState != CharacterWindowState.Equipment)
            {
                UpdateWindowState(CharacterWindowState.Equipment);
            }
        }

        public void StatsState()
        {
            if (_windowState != CharacterWindowState.Stats)
            {
                UpdateWindowState(CharacterWindowState.Stats);
            }
        }

        private void UpdateWindowState(CharacterWindowState state)
        {
            _windowState = state;
            Destroy(_currentTab);
            switch (_windowState)
            {
                case CharacterWindowState.Equipment:
                    var equipmentController = Instantiate(_characterEquipmentTemplate, _content);
                    _currentTab = equipmentController.gameObject;
                    break;
                case CharacterWindowState.Stats:
                    var statsController = Instantiate(_characterStatsTemplate, _content);
                    _currentTab = statsController.gameObject;
                    break;
            }
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}