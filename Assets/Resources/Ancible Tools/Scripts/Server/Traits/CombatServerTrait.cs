using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Combat Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Combat")]
    public class CombatServerTrait : ServerTrait
    {
        [SerializeField] private CombatAlignment _alignment;

        public override TraitData GetData()
        {
            return new CombatTraitData {Alignment = _alignment, MaxStack = 1, Name = name};
        }
    }
}