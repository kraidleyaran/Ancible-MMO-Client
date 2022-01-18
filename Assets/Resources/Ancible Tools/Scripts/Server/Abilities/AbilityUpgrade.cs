using System;
using System.Linq;
using AncibleCoreCommon.CommonData.Ability;
using Assets.Resources.Ancible_Tools.Scripts.Server.Traits;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Abilities
{
    [Serializable]
    public class AbilityUpgrade
    {
        public int Range = 1;
        public int CastTime = 0;
        public ResourceCost[] ResourceCosts;
        public ServerTrait[] ApplyToOwner;
        public ServerTrait[] ApplyToTarget;


        public AbilityUpgradeData GetData()
        {
            return new AbilityUpgradeData
            {
                Range = Range,
                CastTime = CastTime,
                Resources = ResourceCosts,
                ApplyToOwner = ApplyToOwner.Where(t => t).Select(t => t.name).ToArray(),
                ApplyToTarget = ApplyToTarget.Where(t => t).Select(t => t.name).ToArray()

            };
        }
    }
}