using System;
using System.Linq;
using AncibleCoreCommon.CommonData.Talents;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Talents
{
    [CreateAssetMenu(fileName = "Talent", menuName = "Ancible Tools/Server/Talent")]
    public class Talent : ScriptableObject
    {
        public Sprite Icon;
        public string DisplayName;
        [TextArea(3, 10)] public string Description;
        public int UnlockLevel = 0;
        public Talent[] Required;
        public TalentRank[] Ranks;

        public TalentData GetData()
        {
            return new TalentData
            {
                Name = name,
                UnlockLevel = UnlockLevel,
                Ranks = Ranks.Select(r => r.GetData()).ToArray(),
                RequiredTalents = Required.Where(t => t).Select(t => t.name).ToArray()
            };
        }

        public string GetDescription(int rank)
        {
            var description = string.Empty;
            if (Ranks.Length > rank)
            {
                var talent = Ranks[rank];
                var talentDescriptions = talent.ApplyOnRank.Where(t => t).Select(t => t.GetClientDescriptor()).Where(d => !string.IsNullOrEmpty(d)).ToArray();
                for (var i = 0; i < talentDescriptions.Length; i++)
                {
                    if (i == 0)
                    {
                        description = $"{talentDescriptions[i]}";
                    }
                    else
                    {
                        description = $"{description}{Environment.NewLine}{talentDescriptions[i]}";
                    }
                }
            }

            if (UnlockLevel > 0)
            {
                description = $"{description}{Environment.NewLine}Required Level: {UnlockLevel}";
            }

            if (Required.Length > 0)
            {
                description = $"{description}{Environment.NewLine}Requrired Talents:";
                for (var i = 0; i < Required.Length; i++)
                {
                    if (i == 0)
                    {
                        description = $"{description} {Required[i].DisplayName}";
                    }
                    else
                    {
                        description = $"{description}, {Required[i].DisplayName}";
                    }
                }
            }

            if (rank < Ranks.Length - 1)
            {
                description = $"{description}{Environment.NewLine}{Environment.NewLine}Next Rank: {Environment.NewLine}{Ranks[rank + 1].GetClientDescription()}";
            }
            return description;
        }
    }
}