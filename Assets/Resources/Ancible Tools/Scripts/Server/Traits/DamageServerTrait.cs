using System;
using System.Linq;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData;
using AncibleCoreCommon.CommonData.Traits;
using AncibleCoreCommon.CommonData.WorldBonuses;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Server.WorldBonuses;
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
        [SerializeField] private WorldTag[] _tags = new WorldTag[0];

        public override TraitData GetData()
        {
            return new DamageTraitData
            {
                Amount = _amount,
                DamageType = _type,
                BonusMultiplier = _damageBonusMultiplier,
                Name = name,
                MaxStack = 1,
                UseWeaponDamage = _useWeaponDamage,
                Tags = _tags.Where(t => t).Select(t => t.name).ToArray()
            };
        }

        public override string GetClientDescriptor()
        {
            var damageRange = (_useWeaponDamage ? DataController.ActiveCharacter.WeaponDamage + _amount : _amount + new IntNumberRange(0,0));
            var playerBonusDamage = CombatController.CalculateBonusDamage(_type, DataController.ActiveCharacter.BaseStats + DataController.ActiveCharacter.BonusStats) * _damageBonusMultiplier;
            var worldBonuses = DataController.ActiveCharacter.WorldBonuses.Select(WorldBonusFactoryController.GetBonusByName).Where(b => b.Type == WorldBonusType.Damage && b.HasTags(_tags.Select(t => t.name).ToArray())).Select(t => t.GetData()).ToArray();
            var worldBonusTotal = worldBonuses.GetBonusesTotal((int) ((damageRange.Minimum + damageRange.Maximum) / 2f));
            damageRange.Minimum += (int)playerBonusDamage;
            damageRange.Maximum += (int) playerBonusDamage;
            var descriptor = $"Deals {damageRange} {(worldBonusTotal > 0 ? StaticMethods.ApplyColorToText($"(+{worldBonusTotal})", ColorFactoryController.BonusStat) : string.Empty)} {_type} Damage";
            return descriptor;
        }
    }
}