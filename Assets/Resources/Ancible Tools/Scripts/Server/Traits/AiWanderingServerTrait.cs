using AncibleCoreCommon.CommonData;
using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Ai Wandering Server Trait", menuName = "Ancible Tools/Server/Traits/Ai/Ai Wandering")]
    public class AiWanderingServerTrait : ServerTrait
    {
        [SerializeField] private int _wanderRange;
        [SerializeField] [Range(0f, 1f)] private float _chanceToIdle;
        [SerializeField] private IntNumberRange _idleTickRange = new IntNumberRange(0,1);

        public override TraitData GetData()
        {
            return new AiWanderingTraitData {Name = name, MaxStack = 1, WanderRange = _wanderRange, ChanceToIdle = _chanceToIdle, IdleTickRange = _idleTickRange};
        }
    }
}