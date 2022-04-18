using AncibleCoreCommon.CommonData.Traits;
using Assets.Resources.Ancible_Tools.Scripts.Server.WorldBonuses;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Apply World Bonus Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Apply World Bonus")]
    public class ApplyWorldBonusServerTrait : ServerTrait
    {
        [SerializeField] private WorldBonus _bonus = null;
        [SerializeField] private bool _permanent = false;

        public override TraitData GetData()
        {
            return new ApplyWorldBonusTraitData {Name = name, Bonus = _bonus ? _bonus.name : name, Permanent = _permanent, MaxStack = _maxStack};
        }

        public override string GetClientDescriptor()
        {
            return _bonus.GetClientDescription();
        }
    }
}