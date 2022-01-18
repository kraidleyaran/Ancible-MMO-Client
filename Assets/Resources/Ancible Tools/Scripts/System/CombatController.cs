using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.Combat;
using AncibleCoreCommon.CommonData.Traits;
using MessageBusLib;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    public class CombatController : MonoBehaviour
    {
        private static CombatController _instance = null;

        private CombatSettings _settings = null;

        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            SubscribeToMessags();
        }

        public static int CalculateBonusDamage(DamageType type, CombatStats totalStats)
        {
            var damage = 0;
            switch (type)
            {
                case DamageType.Physical:

                    var strengthDamage = (int)(_instance._settings.DamagePerStrength * totalStats.Strength);
                    var agilityDamage = (int)(_instance._settings.DamagePerAgility * totalStats.Agility);
                    damage = strengthDamage + agilityDamage;
                    break;
                case DamageType.Magical:
                    var intelligenceDamage = (int)(_instance._settings.DamagePerIntelligence * totalStats.Intelligence);
                    damage = intelligenceDamage;
                    break;
            }

            return damage;
        }

        public static int CalculateResistedDamage(DamageType type, CombatStats totalStats, int damage)
        {
            var returnDamage = 0;
            switch (type)
            {
                case DamageType.Physical:
                    var strengthDefense = (int)(_instance._settings.DefensePerStrength * totalStats.Strength);
                    var physicalEnduranceDefense = (int)(_instance._settings.PhysicalDefensePerEndurance * totalStats.Endurance);
                    var physicalDefense = totalStats.PhysicalDefense + strengthDefense + physicalEnduranceDefense;
                    if (physicalDefense > damage)
                    {
                        var requiredTotalDefense = damage * _instance._settings.DefenseFallOffMultiplier;
                        var remainingPerecent = 1f - _instance._settings.DefenseFallOff;
                        var additionalPerecent = (physicalDefense / requiredTotalDefense) * remainingPerecent;
                        if (additionalPerecent >= remainingPerecent)
                        {
                            returnDamage = damage;
                        }
                        else
                        {
                            returnDamage = (int)((additionalPerecent + remainingPerecent) * damage);
                        }
                    }
                    else
                    {
                        returnDamage = (int)((float)physicalDefense / damage * _instance._settings.DefenseFallOff);
                    }
                    break;
                case DamageType.Magical:
                    var wisdomDefense = (int)(_instance._settings.MagicalDefensePerWisdom * totalStats.Wisdom);
                    var magicalEnduranceDefense = (int)(_instance._settings.MagicalDefensePerEndurance * totalStats.Endurance);
                    var magicalDefense = totalStats.MagicalDefense + wisdomDefense + magicalEnduranceDefense;
                    if (magicalDefense > returnDamage)
                    {
                        var requiredTotalDefense = damage * _instance._settings.DefenseFallOffMultiplier;
                        var remainingPerecent = 1f - _instance._settings.DefenseFallOff;
                        var additionalPerecent = (magicalDefense / requiredTotalDefense) * remainingPerecent;
                        if (additionalPerecent >= remainingPerecent)
                        {
                            returnDamage = damage;
                        }
                        else
                        {
                            returnDamage = (int)((additionalPerecent + remainingPerecent) * damage);
                        }
                    }
                    else
                    {
                        returnDamage = (int)((float)magicalDefense / damage * _instance._settings.DefenseFallOff);
                    }
                    break;
            }

            return returnDamage;
        }

        public static int CalculateRegenMana(CombatStats totalStats)
        {
            return (int)(_instance._settings.ManaRegenPerWisdom * totalStats.Wisdom);
        }

        public static int CalculateIncomingSpirit(CombatStats totalStats)
        {
            return (int)(_instance._settings.SpiritIncomingRegenPerWisdom * totalStats.Wisdom);
        }

        public static int CalculateOutgoingSpirit(CombatStats totalStats)
        {
            return (int)(_instance._settings.SpiritOutgoingRegenPerWisdom * totalStats.Wisdom);
        }

        public static int CalculateFocusCostReduction(CombatStats totalStats)
        {
            return (int)(_instance._settings.FocusCostPerWisdom * totalStats.Wisdom);
        }

        public static int CalculateAggroPerHeal(int heal)
        {
            var aggro = (int)(heal * _instance._settings.AggroPerHeal);
            if (aggro <= 0)
            {
                aggro = 1;
            }
            return aggro;
        }

        public static int CalculateAggroPerDamage(int damage)
        {
            var aggro = (int)(damage * _instance._settings.AggroPerDamage);
            if (aggro <= 0)
            {
                aggro = 1;
            }
            return aggro;
        }

        public static int CalculateHealBonus(CombatStats totalStats, DamageType type)
        {
            switch (type)
            {
                case DamageType.Physical:
                    return (int)(totalStats.Wisdom * _instance._settings.PhysicalHealPerWisdom);
                case DamageType.Magical:
                    return (int)(totalStats.Intelligence * _instance._settings.MagicalHealPerIntelligence + totalStats.Wisdom * _instance._settings.MagicalHealPerWisdom);
            }

            return 0;
        }

        private void SubscribeToMessags()
        {
            gameObject.Subscribe<ClientCombatSettingsUpdateMessage>(ClientCombatSettingsUpdate);
        }

        private void ClientCombatSettingsUpdate(ClientCombatSettingsUpdateMessage msg)
        {
            _settings = msg.Settings;
        }

        void OnDestroy()
        {
            gameObject.UnsubscribeFromAllMessages();
        }
    }
}