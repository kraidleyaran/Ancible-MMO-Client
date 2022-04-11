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
        public Talent[] Required;
        public TalentRank[] Ranks;

        public TalentData GetData()
        {
            return new TalentData
            {
                Name = name,
                Ranks = Ranks.Select(r => r.GetData()).ToArray(),
                RequiredTalents = Required.Where(t => t).Select(t => t.name).ToArray()
            };
        }
    }
}