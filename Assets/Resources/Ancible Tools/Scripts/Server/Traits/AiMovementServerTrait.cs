using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Ai Movement Server Trait", menuName = "Ancible Tools/Server/Traits/Ai/Ai Movement")]
    public class AiMovementServerTrait : ServerTrait
    {
        [SerializeField] private int _ticksToMove = 1;

        public override TraitData GetData()
        {
            return new AiMovementTraitData {Name = name, MaxStack = 1, TicksToMove = _ticksToMove};
        }
    }
}