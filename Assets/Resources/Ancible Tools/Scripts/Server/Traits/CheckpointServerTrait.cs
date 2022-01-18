using AncibleCoreCommon.CommonData.Traits;
using Assets.Ancible_Tools.Scripts.System;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Checkpoint Server Trait", menuName = "Ancible Tools/Server/Traits/Interactables/Checkpoint")]
    public class CheckpointServerTrait : ServerTrait
    {
        [SerializeField] private Vector2Int _relativePosition;

        public override TraitData GetData()
        {
            return new CheckpointTraitData {Name = name, MaxStack = 1, RelativePosition = _relativePosition.ToData()};
        }
    }
}