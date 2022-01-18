using Assets.Ancible_Tools.Scripts.System;
using MessageBusLib;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.UI.Character
{
    public class UiCharacterCombatStatsController : MonoBehaviour
    {
        [Header("Base Stat References")]
        [SerializeField] private UiCharacterStatController _healthController;
        [SerializeField] private UiCharacterStatController _strengthController;
        [SerializeField] private UiCharacterStatController _agilityController;
        [SerializeField] private UiCharacterStatController _intelligenceController;
        [SerializeField] private UiCharacterStatController _enduranceController;
        [SerializeField] private UiCharacterStatController _wisdomController;
        [SerializeField] private UiCharacterStatController _dexterityController;

        [Header("Secondary Stat References")]
        [SerializeField] private UiCharacterStatController _physicalDefenseController;
        [SerializeField] private UiCharacterStatController _physicalCriticalStrikeController;
        [SerializeField] private UiCharacterStatController _magicalDefenseController;
        [SerializeField] private UiCharacterStatController _magicCriticalStrikeController;
        [SerializeField] private UiCharacterStatController _dodgeRatingController;

        void Awake()
        {
            Refresh();
            SubscribeToMessages();
        }

        private void Refresh()
        {
            var baseStats = DataController.ActiveCharacter.BaseStats;
            var bonusStats = DataController.ActiveCharacter.BonusStats;

            _healthController.Setup(baseStats.Health, bonusStats.Health);
            _strengthController.Setup(baseStats.Strength, bonusStats.Strength);
            _agilityController.Setup(baseStats.Agility, bonusStats.Agility);
            _intelligenceController.Setup(baseStats.Intelligence, bonusStats.Intelligence);
            _enduranceController.Setup(baseStats.Endurance, bonusStats.Endurance);
            _wisdomController.Setup(baseStats.Wisdom, bonusStats.Endurance);
            _dexterityController.Setup(baseStats.Dexterity, bonusStats.Dexterity);
            _physicalDefenseController.Setup(baseStats.PhysicalDefense, bonusStats.Dexterity);
            _physicalCriticalStrikeController.Setup(baseStats.PhysicalCriticalStrike, bonusStats.PhysicalCriticalStrike);
            _magicalDefenseController.Setup(baseStats.MagicalDefense, bonusStats.MagicalDefense);
            _magicCriticalStrikeController.Setup(baseStats.MagicalCriticalStrike, bonusStats.MagicalCriticalStrike);
            _dodgeRatingController.Setup(baseStats.DodgeRating, bonusStats.DodgeRating);
        }

        private void SubscribeToMessages()
        {
            gameObject.Subscribe<RefreshPlayerDataMessage>(RefreshPlayerData);
        }

        private void RefreshPlayerData(RefreshPlayerDataMessage msg)
        {
            Refresh();
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }

    }
}