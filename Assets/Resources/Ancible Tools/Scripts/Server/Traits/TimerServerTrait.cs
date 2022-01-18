using System.Linq;
using AncibleCoreCommon.CommonData.Client;
using AncibleCoreCommon.CommonData.Traits;
using Assets.Ancible_Tools.Scripts.System;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Timer Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Time/Timer")]
    public class TimerServerTrait : ServerTrait
    {
        [SerializeField] private int _timerTicks;
        [SerializeField] private ServerTrait[] _applOnStart = new ServerTrait[0];
        [SerializeField] private ServerTrait[] _applyOnEnd = new ServerTrait[0];
        [SerializeField] private string _status = string.Empty;
        [SerializeField] private bool _damageInterruptable = false;
        [SerializeField] private bool _show = false;
        [SerializeField] private ObjectIconType _iconType = ObjectIconType.Neutral;

        public override TraitData GetData()
        {
            return new TimerTraitData
            {
                Name = name,
                MaxStack = _maxStack,
                TimerTicks = _timerTicks,
                ApplyOnStart = _applOnStart.Where(t => t).Select(t => t.name).ToArray(),
                ApplyOnEnd = _applyOnEnd.Where(t => t).Select(t => t.name).ToArray(),
                Status =  _status.ToLower(),
                DamageInterruptable = _damageInterruptable,
                Display = _show,
                IconType = _iconType

            };
        }

        public override string GetClientDescriptor()
        {
            var desciptor = string.Empty;
            if (_applOnStart.Length > 0)
            {
                var traitDescriptors = _applOnStart.Where(t => t).Select(t => t.GetClientDescriptor()).Where(d => !string.IsNullOrWhiteSpace(d)).ToArray();
                for (var i = 0; i < traitDescriptors.Length; i++)
                {
                    if (i < traitDescriptors.Length - 1)
                    {
                        desciptor = i == 0 ? $"{traitDescriptors[i]} then " : $"{desciptor}{traitDescriptors[i]} then ";
                    }
                    else
                    {
                        
                        desciptor = i == 0 ? $"{traitDescriptors[i]}" : $"{desciptor}{traitDescriptors[i]}";
                    }
                }

                desciptor = $"{desciptor} for {WorldTickController.TickRate / 1000f * _timerTicks} seconds.";
            }

            if (_applyOnEnd.Length > 0)
            {
                desciptor = $"{desciptor}{(string.IsNullOrEmpty(desciptor) ? string.Empty : " ")}After {WorldTickController.TickRate / 1000f * _timerTicks} seconds, ";
                var traitDescriptors = _applyOnEnd.Where(t => t).Select(t => t.GetClientDescriptor()).Where(d => !string.IsNullOrWhiteSpace(d)).ToArray();
                for (var i = 0; i < traitDescriptors.Length; i++)
                {
                    if (i < traitDescriptors.Length - 1)
                    {
                        desciptor = i == 0 ? $"{traitDescriptors[i]} then " : $"{desciptor}{traitDescriptors[i]} then ";
                    }
                    else
                    {
                        desciptor = i == 0 ? $"{traitDescriptors[i]}" : $"{desciptor}{traitDescriptors[i]}.";
                    }
                }
            }
            if (_maxStack > 1)
            {
                desciptor = $"{desciptor} Stacks up to {_maxStack} times.";
            }
            return desciptor;
        }
    }
}