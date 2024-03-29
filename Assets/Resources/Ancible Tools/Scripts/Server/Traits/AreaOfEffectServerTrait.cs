﻿using System.Linq;
using AncibleCoreCommon.CommonData.Ability;
using AncibleCoreCommon.CommonData.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Area of Effect Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Area of Effect")]
    public class AreaOfEffectServerTrait : ServerTrait
    {
        [SerializeField] private int _area = 1;
        [SerializeField] private ServerTrait[] _applyToTargets;
        [SerializeField] private AbilityAlignment _alignmentRequirement = AbilityAlignment.All;
        [SerializeField] private int _targetCount = 0;

        public override TraitData GetData()
        {
            return new AreaOfEffectTraitData
            {
                Name = name,
                MaxStack = _maxStack,
                Area = _area,
                ApplyToTargets = _applyToTargets.Where(t => t).Select(t => t.name).ToArray(),
                AlignmentRequirement = _alignmentRequirement,
                TargetCount = _targetCount
            };
        }

        public override string GetClientDescriptor()
        {
            var description = string.Empty;
            var traitDescriptors = _applyToTargets.Select(t => t.GetClientDescriptor()).Where(d => !string.IsNullOrEmpty(d)).ToArray();
            for (var i = 0; i < traitDescriptors.Length; i++)
            {
                if (i == 0)
                {
                    description = $"{traitDescriptors[i]}";
                }
                else if (i < traitDescriptors.Length - 1)
                {
                    description = $"{description}, {traitDescriptors}";
                }
                else
                {
                    description = $"{description} and {traitDescriptors[i]}";
                }
            }
            description = $"{description} to {(_targetCount > 0 ? $"{_targetCount}" : "all")} targets in a {_area} tile radius";
            return description;
        }
    }
}