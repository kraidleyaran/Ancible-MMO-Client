using System.Linq;
using AncibleCoreCommon.CommonData.Client;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Ancible_Tools.Scripts.System;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Over Time Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Time/Over Time")]
    public class OverTimeServerTrait : ServerTrait
    {
        [SerializeField] private int _timerTicks;
        [SerializeField] private int _loops;
        [SerializeField] private ServerTrait[] _applyOnLoopComplete = new ServerTrait[0];
        [SerializeField] private bool _applyOnStart;
        [SerializeField] private string _status = string.Empty;
        [SerializeField] private bool _damageInterruptable = false;
        [SerializeField] private bool _show = false;
        [SerializeField] private ObjectIconType _iconType = ObjectIconType.Neutral;

        public override TraitData GetData()
        {
            return new OverTimeTraitData
            {
                Name = name,
                MaxStack = _maxStack,
                TicksToComplete = _timerTicks,
                Loops = _loops,
                ApplyOnLoopComplete = _applyOnLoopComplete.Where(t => t).Select(t => t.name).ToArray(),
                ApplyOnStart = _applyOnStart,
                Status = _status.ToLower(),
                DamageInterruptable = _damageInterruptable,
                Display = _show,
                IconType = _iconType
            };
        }

        public override string GetClientDescriptor()
        {
            var desciptor = string.Empty;
            var traitDescriptors = _applyOnLoopComplete.Where(t => t).Select(t => t.GetClientDescriptor()).Where(d => !string.IsNullOrWhiteSpace(d)).ToArray();
            for (var i = 0; i < traitDescriptors.Length; i++)
            {
                if (i < traitDescriptors.Length - 1)
                {
                    desciptor = i == 0 ? $"{traitDescriptors[i]}," : $"{desciptor}{traitDescriptors[i]}";
                }
                else
                {
                    desciptor = $"{desciptor}{traitDescriptors[i]}";
                }
            }

            desciptor = $"{desciptor} every {WorldTickController.TickRate / 1000f * _timerTicks } seconds for {WorldTickController.TickRate / 1000f * _timerTicks * _loops} seconds.";
            if (_maxStack > 1)
            {
                desciptor = $"{desciptor} Stacks up to {_maxStack} times.";
            }
            return desciptor;
        }
    }
}