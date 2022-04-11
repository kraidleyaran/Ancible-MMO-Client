using System;
using System.Linq;
using AncibleCoreCommon.CommonData;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Server.WorldBonuses;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Heal Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Heal")]
    public class HealServerTrait : ServerTrait
    {
        [SerializeField] private IntNumberRange _amount = new IntNumberRange {Minimum = 1, Maximum = 2};
        [SerializeField] private DamageType _healType = DamageType.Physical;
        [SerializeField] private bool _applyBonus = false;
        [SerializeField] private bool _broadcast = false;
        [SerializeField] private WorldTag[] _tags = new WorldTag[0];

        public override TraitData GetData()
        {
            return new HealTraitData
            {
                Name = name,
                MaxStack = _maxStack,
                Amount = _amount,
                DamageType = _healType,
                ApplyBonus = _applyBonus,
                Broadcast = _broadcast,
                Tags = _tags.Where(t => t).Select(t => t.name).ToArray()
            };
        }

        public override string GetClientDescriptor()
        {
            var amount = _amount + new IntNumberRange(0,0);
            if (_applyBonus)
            {
                amount.Minimum += CombatController.CalculateHealBonus(DataController.ActiveCharacter.BaseStats + DataController.ActiveCharacter.BonusStats, _healType);
                amount.Maximum += CombatController.CalculateHealBonus(DataController.ActiveCharacter.BaseStats + DataController.ActiveCharacter.BonusStats, _healType);
            }
            var description = $"Heals for {amount} health";

            return description;
        }
    }
}