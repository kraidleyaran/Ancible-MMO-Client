using System;
using System.Linq;
using AncibleCoreCommon.CommonData.Ability;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Server.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Abilities
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ancible Tools/Server/Ability")]
    public class Ability : ScriptableObject
    {
        public string DisplayName;
        public string PositionName;
        public bool Auto;
        public Sprite Icon;
        [TextArea(3, 10)] public string Description;
        public int Range = 1;
        public int CastTime = 0;
        public int Cooldown = 0;
        public AbilityAlignment Alignment;
        public TargetType TargetType;
        public ResourceCost[] ResourceCosts;
        public ServerTrait[] ApplyToOwner;
        public ServerTrait[] ApplyToTarget;
        public AbilityUpgrade[] Upgrades;

        public AbilityData GetData()
        {
            return new AbilityData
            {
                Name = name,
                PositionName = PositionName,
                Range = Range,
                CastTime = CastTime,
                TargetAlignment = Alignment,
                TargetType = TargetType,
                Resources = ResourceCosts,
                ApplyToOwner = ApplyToOwner.Where(t => t).Select(t => t.name).ToArray(),
                ApplyToTarget = ApplyToTarget.Where(t => t).Select(t => t.name).ToArray(),
                Upgrades = Upgrades.Where(u => u != null).Select(u => u.GetData()).ToArray(),
                Cooldown =  Cooldown
            };
        }

        public string GetClientDescription(int rank = 0)
        {
            var description = string.Empty;
            description = $"Rank:{rank + 1}";
            description = $"{description}{Environment.NewLine}Cast Time:{CastTime * (WorldTickController.TickRate / 1000f):N} seconds";
            description = $"{description}{Environment.NewLine}Cooldown:{Cooldown * (WorldTickController.TickRate / 1000f):N} seconds";
            description = $"{description}{Environment.NewLine}Target:{TargetType}";
            description = $"{description}{Environment.NewLine}Alignment:{Alignment}";

            
            var ownerTraitDescriptors = rank > 0 ? Upgrades[rank - 1].ApplyToOwner.Where(t => t).Select(t => t.GetClientDescriptor()).Where(t => !string.IsNullOrEmpty(t)).ToArray() : ApplyToOwner.Where(t => t).Select(t => t.GetClientDescriptor()).Where(t => !string.IsNullOrEmpty(t)).ToArray();
            if (ownerTraitDescriptors.Length > 0)
            {
                var ownerDescription = string.Empty;
                for (var i = 0; i < ownerTraitDescriptors.Length; i++)
                {
                    if (i < ownerTraitDescriptors.Length - 1)
                    {
                        ownerDescription = i == 0 ? $"{ownerTraitDescriptors[i]} then " : $"{ownerDescription}{ownerTraitDescriptors[i]} then ";
                    }
                    else
                    {
                        ownerDescription = i == 0 ? $"{ownerTraitDescriptors[i]}" : $"{ownerDescription}{ownerTraitDescriptors[i]}.";
                    }
                }

                ownerDescription = $"{ownerDescription} to self";
                description = $"{description}{Environment.NewLine}{ownerDescription}";
            }

            var targetTraitDescriptors = rank > 0 ? Upgrades[rank - 1].ApplyToTarget.Where(t => t).Select(t => t.GetClientDescriptor()).Where(t => !string.IsNullOrEmpty(t)).ToArray() : ApplyToTarget.Where(t => t).Select(t => t.GetClientDescriptor()).Where(t => !string.IsNullOrEmpty(t)).ToArray();
            if (targetTraitDescriptors.Length > 0)
            {
                var targetDescription = string.Empty;
                for (var i = 0; i < targetTraitDescriptors.Length; i++)
                {
                    if (i < targetTraitDescriptors.Length - 1)
                    {
                        targetDescription = i == 0 ? $"{targetTraitDescriptors[i]}," : $"{targetDescription}{targetTraitDescriptors[i]},";
                    }
                    else
                    {
                        targetDescription = $"{targetDescription}{targetTraitDescriptors[i]}";
                    }
                }

                targetDescription = $"{targetDescription} to target";
                description = $"{description}{Environment.NewLine}{targetDescription}";
            }
            if (ResourceCosts.Length > 0)
            {
                var costDescription = string.Empty;
                for (var i = 0; i < ResourceCosts.Length; i++)
                {
                    costDescription = i < ResourceCosts.Length - 1 ? $"{costDescription}{ResourceCosts[i].Amount} {ResourceCosts[i].Type}," : $"{costDescription}{ResourceCosts[i].Amount} {ResourceCosts[i].Type}";
                }

                description = $"{description}{Environment.NewLine}{costDescription}";
            }
            return description;
        }
    }
}