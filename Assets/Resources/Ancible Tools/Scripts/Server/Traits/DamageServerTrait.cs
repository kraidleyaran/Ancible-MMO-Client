using System;
using AncibleCoreCommon.CommonData;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Ancible_Tools.Scripts.System;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Damage Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Damage")]
    public class DamageServerTrait : ServerTrait
    {
        [SerializeField] private IntNumberRange _amount;
        [SerializeField] private DamageType _type;
        [SerializeField] private float _damageBonusMultiplier = 1f;
        [SerializeField] private bool _useWeaponDamage = false;

        public override TraitData GetData()
        {
            return new DamageTraitData {Amount = _amount, DamageType = _type, BonusMultiplier = _damageBonusMultiplier, Name = name, MaxStack = 1, UseWeaponDamage = _useWeaponDamage};
        }

        public override string GetClientDescriptor()
        {
            var damageRange = (_useWeaponDamage ? DataController.ActiveCharacter.WeaponDamage + _amount : _amount + new IntNumberRange(0,0));
            var playerBonusDamage = CombatController.CalculateBonusDamage(_type, DataController.ActiveCharacter.BaseStats + DataController.ActiveCharacter.BonusStats) * _damageBonusMultiplier;
            damageRange.Minimum += (int)playerBonusDamage;
            damageRange.Maximum += (int) playerBonusDamage;
            
            var descriptor = $"Deals {damageRange} {_type} Damage";
            return descriptor;
        }
    }
}