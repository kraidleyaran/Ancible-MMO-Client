using System.Linq;
using AncibleCoreCommon.CommonData.WorldBonuses;
using UnityEngine;

namespace Assets.Resources.Ancible_Tools.Scripts.Server.WorldBonuses
{
    [CreateAssetMenu(fileName = "World Bonus", menuName = "Ancible Tools/Server/World Bonuses/World Bonus")]
    public class WorldBonus : ScriptableObject
    {
        [SerializeField] private WorldBonusType _type;
        [SerializeField] private int _amount = 0;
        [SerializeField] private WorldTag[] _tags = new WorldTag[0];

        public WorldBonusData GetData()
        {
            return new WorldBonusData
            {
                Name = name,
                Amount = _amount,
                Tags = _tags.Where(t => t).Select(t => t.name).ToArray(),
                Type = _type
            };
        }
    }
}