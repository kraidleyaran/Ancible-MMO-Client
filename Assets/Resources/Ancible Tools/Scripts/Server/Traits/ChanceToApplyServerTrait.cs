using System;
using System.Linq;
using AncibleCoreCommon;
using AncibleCoreCommon.CommonData.Traits;
using AncibleCoreCommon.CommonData.WorldBonuses;
using Assets.Ancible_Tools.Scripts.System;
using Assets.Resources.Ancible_Tools.Scripts.Server.WorldBonuses;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Traits
{
    [CreateAssetMenu(fileName = "Chance to Apply Server Trait", menuName = "Ancible Tools/Server/Traits/Combat/Chance to Apply")]
    public class ChanceToApplyServerTrait : ServerTrait
    {
        [SerializeField] [Range(0f, 1f)] private float _chanceToApply;
        [SerializeField] private ServerTrait[] _applyOnChance = new ServerTrait[0];
        [SerializeField] private WorldTag[] _tags = new WorldTag[0];

        public override TraitData GetData()
        {
            return new ChanceToApplyTraitData
            {
                Name = name,
                MaxStack = 0,
                ChanceToApply = _chanceToApply,
                ApplyOnChance = _applyOnChance.Where(t => t).Select(t => t.name).ToArray(),
                Tags = _tags.Where(t => t).Select(t => t.name).ToArray()
            };
        }

        public override string GetClientDescriptor()
        {
            var chanceToApply = _chanceToApply;
            var worldBonuses = DataController.ActiveCharacter.WorldBonuses.Select(WorldBonusFactoryController.GetBonusByName).Where(b => b.Type == WorldBonusType.Chance && b.HasTags(_tags.Select(t => t.name).ToArray())).Select(b => b.GetData()).ToArray();
            chanceToApply += worldBonuses.GetChanceBonusesTotal(_chanceToApply);
            var descriptor = $"{Environment.NewLine}{StaticMethods.ApplyColorToText($"{chanceToApply:P}% Chance To Apply:", ColorFactoryController.BonusStat)}";
            var traitDescriptors = _applyOnChance.Select(t => t.GetClientDescriptor()).Where(d => !string.IsNullOrEmpty(d)).ToArray();
            for (var i = 0; i < traitDescriptors.Length; i++)
            {
                if (i == 0)
                {
                    descriptor = $"{descriptor} {traitDescriptors[i]}";
                }
                else if (i < traitDescriptors.Length)
                {
                    descriptor = $"{descriptor}, {traitDescriptors[i]}";
                }
                else
                {
                    descriptor = $"{descriptor} and {traitDescriptors[i]}";
                }
            }

            return descriptor;
        }
    }
}