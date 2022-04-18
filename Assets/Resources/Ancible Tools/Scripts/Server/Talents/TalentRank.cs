using System;
using System.Linq;
using AncibleCoreCommon.CommonData.Talents;
using Assets.Resources.Ancible_Tools.Scripts.Server.Traits;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.Talents
{
    [Serializable]
    public class TalentRank
    {
        public ServerTrait[] ApplyOnRank => _applyOnRank;
        [TextArea(3, 10)] public string Description;
        [SerializeField] private ServerTrait[] _applyOnRank;

        public TalentRankData GetData()
        {
            return new TalentRankData {ApplyOnRank = _applyOnRank.Where(t => t).Select(t => t.name).ToArray()};
        }

        public string GetClientDescription()
        {
            var description = Description;
            var traitDescriptions = _applyOnRank.Where(t => t).Select(t => t.GetClientDescriptor()).Where(d => !string.IsNullOrEmpty(d)).ToArray();
            if (traitDescriptions.Length > 0)
            {
                if (!string.IsNullOrEmpty(description))
                {
                    description = $"{description}{Environment.NewLine}";
                }

                if (traitDescriptions.Length > 0)
                {
                    description = $"{description}{traitDescriptions[0]}";
                }
                else
                {
                    for (var i = 0; i < traitDescriptions.Length; i++)
                    {
                        if (i == 0)
                        {
                            description = $"{description}{traitDescriptions[i]}";
                        }
                        else if (i < traitDescriptions.Length - 1)
                        {
                            description = $"{description}, {traitDescriptions[i]}";
                        }
                        else
                        {
                            description = $"{description}, and {traitDescriptions[i]}";
                        }
                    }
                }

            }
            return description;
        }
    }
}