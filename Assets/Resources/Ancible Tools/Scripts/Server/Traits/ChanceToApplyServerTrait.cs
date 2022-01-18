using System.Linq;
using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Chance to Apply Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Chance to Apply")]
    public class ChanceToApplyServerTrait : ServerTrait
    {
        [SerializeField] [Range(0f, 1f)] private float _chanceToApply;
        [SerializeField] private ServerTrait[] _applyOnChance = new ServerTrait[0];

        public override TraitData GetData()
        {
            return new ChanceToApplyTraitData
            {
                Name = name,
                MaxStack = 0,
                ChanceToApply = _chanceToApply,
                ApplyOnChance = _applyOnChance.Where(t => t).Select(t => t.name).ToArray()
            };
        }
    }
}