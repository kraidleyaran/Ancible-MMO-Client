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
        [SerializeField] private ServerTrait[] _applyOnRank;

        public TalentRankData GetData()
        {
            return new TalentRankData {ApplyOnRank = _applyOnRank.Where(t => t).Select(t => t.name).ToArray()};
        }
    }
}