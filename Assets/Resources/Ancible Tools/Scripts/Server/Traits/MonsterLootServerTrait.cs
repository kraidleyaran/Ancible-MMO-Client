using AncibleCoreCommon.CommonData;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Resources.Ancible_Tools.Scripts.Server.Items;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Monster Loot Server Trait", menuName = "Ancible Tools/Server/Traits/Loot/Monster Loot")]
    public class MonsterLootServerTrait : ServerTrait
    {
        [SerializeField] private LootTable _lootTable;
        [SerializeField] private IntNumberRange _experience;

        public override TraitData GetData()
        {
            return new MonsterLootTraitData {Name = name, LootTable = _lootTable.name, Experience = _experience, MaxStack = 1};
        }
    }
}