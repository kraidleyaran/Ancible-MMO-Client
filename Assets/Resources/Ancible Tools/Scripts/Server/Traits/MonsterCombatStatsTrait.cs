using AncibleCoreCommon.CommonData.Combat;
using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Monster Combat Stats Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Monster Combat Stats")]
    public class MonsterCombatStatsTrait : ServerTrait
    {
        [SerializeField] private MonsterCombatStats _stats;

        public override TraitData GetData()
        {
            return new MonsterCombatStatsTraitData {Name = name, MaxStack = 1, Stats = _stats};
        }
    }
}