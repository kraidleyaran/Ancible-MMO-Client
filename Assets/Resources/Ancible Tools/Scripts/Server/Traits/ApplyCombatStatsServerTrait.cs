using AncibleCoreCommon.CommonData.Combat;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Ancible_Tools.Scripts.System;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Apply Combat Stats Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Apply Combat Stats")]
    public class ApplyCombatStatsServerTrait : ServerTrait
    {
        [SerializeField] private CombatStats _stats;

        public override TraitData GetData()
        {
            return new ApplyCombatStatsTraitData {MaxStack = _maxStack, Name = name, Stats = _stats};
        }

        public override string GetClientDescriptor()
        {
            return _stats.ToDescription();
        }
    }
}