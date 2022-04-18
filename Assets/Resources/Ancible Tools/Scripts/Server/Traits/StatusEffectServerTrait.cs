using AncibleCoreCommon.CommonData.Combat;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Ancible_Tools.Scripts.System;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Status Effect Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Status Effect")]
    public class StatusEffectServerTrait : ServerTrait
    {
        [SerializeField] private StatusEffectType _type;
        [SerializeField] private int _length = 0;

        public override TraitData GetData()
        {
            return new StatusEffectTraitData {EffectType = _type, Length = _length, MaxStack = 1, Name = name};
        }

        public override string GetClientDescriptor()
        {
            return $"Applies a {_type} for {WorldTickController.TickRate / 1000f * _length} seconds";
        }
    }
}