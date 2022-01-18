using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Ai Aggro Server Trait", menuName = "Ancible Tools/Server/Traits/Ai/Ai Aggro")]
    public class AiAggroServerTrait : ServerTrait
    {
        [SerializeField] private int _aggroRange;
        [SerializeField] private int _aggroTickChecks;
        [SerializeField] private bool _healOnAggroDrop;

        public override TraitData GetData()
        {
            return new AiAggroTraitData { Name = name, MaxStack = 1, AggroRange = _aggroRange, AggroCheckTicks = _aggroTickChecks, HealOnAggroDrop = _healOnAggroDrop};
        }
    }
}